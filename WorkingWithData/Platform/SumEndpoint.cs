using Microsoft.Extensions.Caching.Distributed;
using WorkingWithData.Contexts;

namespace WorkingWithData.Platform
{
    public class SumEndpoint
    {
        public async Task Endpoint(HttpContext context, CalculationContext dbContext)
        {
            int count
                ;
            int.TryParse((string?)context.Request.RouteValues["count"], out count);

            long total = dbContext.Calculations?.FirstOrDefault(c => c.Count == count)?.Result ?? 0;

            if(total == 0)
            {
                for(int i = 1; i <= count; i++)
                {
                    total += i;
                }

                dbContext.Calculations?.Add(new()
                {
                    Count = count,
                    Result = total
                });

                await dbContext.SaveChangesAsync();
            }


            string totalString = $"({DateTime.Now.ToLongTimeString()}) {total}";

            await context.Response.WriteAsync($"({DateTime.Now.ToLongTimeString}) Total for {count}"
                + $" values: \n{totalString}\n");

        }
    }
}

#region Caching data values
/*
 public async Task Endpoint(HttpContext context, IDistributedCache cache)
        {
            int count;

            int.TryParse((string?)context.Request.RouteValues["count"], out count);

            string cacheKey = $"sum_{count}";

            string totalString = await cache.GetStringAsync(cacheKey);

            if(totalString == null)
            {
                long total = 0;

                for(int i = 1; i <= count; i++)
                {
                    total += i;
                }

                totalString = $"({DateTime.Now.ToLongTimeString()}) {total}";

                await cache.SetStringAsync(cacheKey, totalString,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                    });
            }

            await context.Response.WriteAsync($"({DateTime.Now.ToLongTimeString()}) Total for {count}" 
                + $" values:\n{totalString}\n"); 
        }
    }
 */
#endregion

#region Caching responses -> ao dar um refresh no navegador, é passado um novo header e a resposta é alterada, por isso, a unica forma de ver é dando reload pelo html
/*
 public class SumEndpoint
    {
        public async Task Endpoint(HttpContext context, IDistributedCache cache)
        {
            int count
                ;
            int.TryParse((string?)context.Request.RouteValues["count"], out count);

            long total = 0;

            for(int i = 1; i <= count; i++)
            {
                total += i;
            }

            string totalString = $"({DateTime.Now.ToLongTimeString()}) {total}";

            context.Response.Headers["Cache-Control"] = "public, max-age=120";

            await context.Response.WriteAsync($"({DateTime.Now.ToLongTimeString()}) {count}"
                + $"\n({totalString})");

        }
    }
 */
#endregion
