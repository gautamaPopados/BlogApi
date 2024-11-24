using WebApplication1.Data.Entities;

namespace WebApplication1.Data
{
    public class DatabaseInitilaizer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                if (!context.Communities.Any())
                {
                    var communitiesToAdd = new List<Community>
                    {
                        new Community
                        {
                            Id = new Guid(),
                            Name = "Приверженцы Теории Большого Свага",
                            CommunityUsers = new List<CommunityUser>(),
                            CreateTime = DateTime.UtcNow,
                        },
                        new Community
                        {
                            Id = new Guid(),
                            Name = "Почему ты ещё не фанат?",
                            Description =  "Неужели не видишь таланта",
                            CommunityUsers = new List<CommunityUser>(),
                            CreateTime = DateTime.UtcNow,
                        },
                        new Community
                        {
                            Id = new Guid(),
                            Name = "Группааа",
                            CommunityUsers = new List<CommunityUser>(),
                            CreateTime = DateTime.UtcNow,
                        },
                    };

                    context.Communities.AddRange(communitiesToAdd);
                    context.SaveChanges();
                }
            }
        }
    }
}
