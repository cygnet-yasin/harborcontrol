using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models;

namespace Harbor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HarborController : ControllerBase
    {
        IMemoryCache _cache;
        public HarborController(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        /// <summary>
        /// First attemt to check memory cache
        /// </summary>
        /// <param name="initialParameter"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public ActionResult FirstAttemt(InitialParameter initialParameter)
        {
            Response response = new Response();
            try
            {
                if (!string.IsNullOrEmpty(initialParameter.token))
                {
                    object cacheValue = GetFromCache(initialParameter.token);
                    if (cacheValue != null)
                    {
                        if (cacheValue is InitialParameter) response.statusCode = StatusCodes.Status100Continue;
                        if (cacheValue is StatusOfHarbor) response.statusCode = StatusCodes.Status200OK;
                        response.data = cacheValue;
                        return Ok(response);
                    }
                }
                InitialParameter newInitialParameter = new InitialParameter();
                newInitialParameter.token = SetUpdateCache(string.Empty, newInitialParameter);
                response.statusCode = StatusCodes.Status100Continue;
                response.data = newInitialParameter;
                SetUpdateCache(newInitialParameter.token, newInitialParameter);
            }
            catch (Exception ex)
            {
                response.statusCode = StatusCodes.Status500InternalServerError;
                response.errorMSG = ex.Message;
                response.errorType = ex.Source;
            }
            return Ok(response);
        }

        /// <summary>
        /// set initial parameter
        /// </summary>
        /// <param name="initialParameter"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public ActionResult SetInitial(InitialParameter initialParameter)
        {
            Response response = new Response();
            try
            {
                List<BoatStatus> boatStatuses = new List<BoatStatus>();
                for (int i = 0; i < initialParameter.boatCount; i++)
                {
                    boatStatuses.Add(GenerateBoats());
                }
                initialParameter.nextAuotGeneratedBoatTime = DateTime.Now.AddSeconds(initialParameter.autoGenerateBoatTime);

                TakeActionOnBoats(boatStatuses, initialParameter);
                StatusOfHarbor statusOfHarbor = new StatusOfHarbor();
                statusOfHarbor.boatStatus = boatStatuses;
                statusOfHarbor.initialParameter = initialParameter;
                response.statusCode = StatusCodes.Status200OK;
                response.data = statusOfHarbor;
                SetUpdateCache(statusOfHarbor.initialParameter.token, statusOfHarbor);
            }
            catch (Exception ex)
            {
                response.statusCode = StatusCodes.Status500InternalServerError;
                response.errorMSG = ex.Message;
                response.errorType = ex.Source;
            }
            return Ok(response);
        }

        /// <summary>
        /// update status of boat
        /// </summary>
        /// <param name="statusOfHarbor"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public ActionResult UpdateStatus(StatusOfHarbor statusOfHarbor)
        {
            Response response = new Response();
            try
            {
                TakeActionOnBoats(statusOfHarbor.boatStatus, statusOfHarbor.initialParameter);
                response.statusCode = StatusCodes.Status200OK;
                response.data = statusOfHarbor;
                SetUpdateCache(statusOfHarbor.initialParameter.token, statusOfHarbor);
            }
            catch (Exception ex)
            {
                response.statusCode = StatusCodes.Status500InternalServerError;
                response.errorMSG = ex.Message;
                response.errorType = ex.Source;
            }
            return Ok(response);
        }

        /// <summary>
        /// set and update cache memory
        /// </summary>
        /// <param name="token"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private string SetUpdateCache(string token, object value)
        {
            if (token == null || string.IsNullOrEmpty(token)) token = Guid.NewGuid().ToString();
            MemoryCacheEntryOptions memoryCacheEntryOptions = new MemoryCacheEntryOptions();
            memoryCacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddMinutes(20);
            memoryCacheEntryOptions.Priority = CacheItemPriority.Normal;
            _cache.Set(token, value, memoryCacheEntryOptions);
            return token;
        }

        /// <summary>
        /// Get data from cache
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private object GetFromCache(string token)
        {
            object cacheValue = null;
            if (_cache != null) _cache.TryGetValue(token, out cacheValue);
            return cacheValue;
        }

        /// <summary>
        /// Generate random boat
        /// </summary>
        /// <returns></returns>
        private BoatStatus GenerateBoats()
        {
            BoatStatus boatStatus = new BoatStatus();

            Array values = Enum.GetValues(typeof(Boats));
            boatStatus.boats = (Boats)values.GetValue(GenerateRandom(0, values.Length));
            boatStatus.boatPosition = BoatPosition.AtPerimeter;
            boatStatus.estimateToReachNextPosition = null;
            return boatStatus;
        }

        /// <summary>
        /// take action on generated boat
        /// </summary>
        /// <param name="boatStatuses"></param>
        /// <param name="initialParameter"></param>
        private void TakeActionOnBoats(List<BoatStatus> boatStatuses, InitialParameter initialParameter)
        {
            initialParameter.windSpeed = GenerateRandom(0, 60);
            if (initialParameter.nextAuotGeneratedBoatTime <= DateTime.Now)
            {
                boatStatuses.Add(GenerateBoats());
                initialParameter.nextAuotGeneratedBoatTime = DateTime.Now.AddSeconds(initialParameter.autoGenerateBoatTime * initialParameter.oneHourPerSecond);
            }

            List<BoatStatus> InComingBoats = boatStatuses.Where(b => b.boatPosition.Equals(BoatPosition.InComing)).ToList();
            if (InComingBoats.Any())
            {
                foreach (BoatStatus inComingBoat in InComingBoats)
                {
                    if (inComingBoat.estimateToReachNextPosition <= DateTime.Now)
                    {
                        inComingBoat.boatPosition = BoatPosition.Anchored;
                        inComingBoat.estimateToReachNextPosition = DateTime.Now.AddSeconds(initialParameter.anchorTime * initialParameter.oneHourPerSecond);
                    }
                }
            }

            List<BoatStatus> OutGoingBoats = boatStatuses.Where(b => b.boatPosition.Equals(BoatPosition.OutGoing)).ToList();
            if (OutGoingBoats.Any())
            {
                foreach (BoatStatus outGoingBoat in OutGoingBoats)
                {
                    if (outGoingBoat.estimateToReachNextPosition <= DateTime.Now)
                    {
                        outGoingBoat.boatPosition = BoatPosition.Shipped;
                        outGoingBoat.estimateToReachNextPosition = DateTime.Now;
                    }
                }
            }

            if (boatStatuses.Where(b => b.boatPosition.Equals(BoatPosition.InComing) || b.boatPosition.Equals(BoatPosition.OutGoing)).Any()) return;

            int setPoint = -1;
            int anchoredCount = boatStatuses.Where(b => b.boatPosition.Equals(BoatPosition.Anchored)).Count();

            if (initialParameter.anchorSize >= anchoredCount)
            {
                if ((initialParameter.anchorSize / 2) > anchoredCount)
                {
                    setPoint = 0;
                }
                else
                {
                    setPoint = 1;
                }
            }
            if (setPoint == 1) setPoint = GenerateRandom(0, 2);
            bool isOpenForSail = initialParameter.windSpeed < 30 && initialParameter.windSpeed > 10;
            switch (setPoint)
            {
                case 0:
                    List<BoatStatus> AtPerimeterBoats = isOpenForSail ? boatStatuses.Where(b => b.boatPosition.Equals(BoatPosition.AtPerimeter)).ToList() : boatStatuses.Where(b => b.boatPosition.Equals(BoatPosition.AtPerimeter) && b.boats != Boats.SailBoat).ToList();
                    if (AtPerimeterBoats.Any())
                    {
                        int inComingBoatIndex = GenerateRandom(0, AtPerimeterBoats.Count);
                        BoatStatus boatStatus = AtPerimeterBoats[inComingBoatIndex];
                        boatStatus.boatPosition = BoatPosition.InComing;
                        boatStatus.estimateToReachNextPosition = DateTime.Now.AddSeconds(CalculateTime(initialParameter.perimeterLineDistance, boatStatus.boats) * initialParameter.oneHourPerSecond);
                    }
                    break;
                case 1:
                    List<BoatStatus> AnchoredBoats = isOpenForSail ? boatStatuses.Where(b => b.boatPosition.Equals(BoatPosition.Anchored)).ToList() : boatStatuses.Where(b => b.boatPosition.Equals(BoatPosition.Anchored) && b.boats != Boats.SailBoat).ToList();
                    if (AnchoredBoats.Any())
                    {
                        int outGoingBoatIndex = GenerateRandom(0, AnchoredBoats.Count);
                        BoatStatus boatStatus = AnchoredBoats[outGoingBoatIndex];
                        boatStatus.boatPosition = BoatPosition.OutGoing;
                        boatStatus.estimateToReachNextPosition = DateTime.Now.AddSeconds(CalculateTime(initialParameter.perimeterLineDistance, boatStatus.boats) * initialParameter.oneHourPerSecond);
                    }
                    break;
            }
        }

        /// <summary>
        /// calculate time with distance and boat speed
        /// </summary>
        /// <param name="Distance"></param>
        /// <param name="boat"></param>
        /// <returns></returns>
        private double CalculateTime(int Distance, Boats boat)
        {
            SpeedOfBoats speedOfBoat;
            if (Enum.TryParse(boat.ToString(), out speedOfBoat))
            {
                return Distance / Convert.ToInt32(speedOfBoat);
            }
            return -1;
        }

        /// <summary>
        /// generate random from given range
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private int GenerateRandom(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
    }
}