using Microsoft.EntityFrameworkCore;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data;
using WebApplication1.Data.DTO;
using WebApplication1.Data.Enums;
using WebApplication1.Services.IServices;

namespace WebApplication1.Services
{
    public class AddressService : IAddressService
    {
        private readonly AddressContext _db;

        public AddressService(AddressContext db, IConfiguration configuration)
        {
            _db = db;
        }

        public async Task<List<SearchAddressModel>> Search(long parentObjectId, string? query)
        {
            var addressses = new List<SearchAddressModel>();

            var childHierarchies = await _db.Hierarchies.Where(h => h.parentobjid == parentObjectId && h.isactive == 1).ToListAsync();

            if (childHierarchies.Any())
            {
                var childObjectIds = childHierarchies.Select(h => h.objectid).ToList();

                var filteredAddresses = _db.AddrElements.Where(a => childObjectIds.Contains(a.objectid)); 

                if (!string.IsNullOrEmpty(query))
                {
                    filteredAddresses = filteredAddresses.Where(a => a.name.Contains(query)); 
                }

                addressses = await filteredAddresses
                    .Select(a => new SearchAddressModel
                    {
                        objectId = a.objectid,
                        objectGuid = a.objectguid,
                        text = a.typename + " " + a.name,
                        objectLevel = (GarAddressLevel)Int32.Parse(a.level),
                        objectLevelText = GetText((GarAddressLevel)Int32.Parse(a.level))
                    })
                    .ToListAsync();

                var houses = _db.Houses.Where(h => childObjectIds.Contains(h.objectid));
            }

            return addressses;
        }

        private static readonly Dictionary<GarAddressLevel, string> LevelTexts = new()
        {
            { GarAddressLevel.Region, "Регион" },
            { GarAddressLevel.AdministrativeArea, "Административный район" },
            { GarAddressLevel.MunicipalArea, "Муниципальный район" },
            { GarAddressLevel.RuralUrbanSettlement, "Сельское или городское поселение" },
            { GarAddressLevel.City, "Город" },
            { GarAddressLevel.Locality, "Населённый пункт" },
            { GarAddressLevel.ElementOfPlanningStructure, "Элемент планировочной структуры" },
            { GarAddressLevel.ElementOfRoadNetwork, "Элемент улично-дорожной сети" },
            { GarAddressLevel.Land, "Земельный участок" },
            { GarAddressLevel.Building, "Здание (сооружение)" },
            { GarAddressLevel.Room, "Помещение" },
            { GarAddressLevel.RoomInRooms, "Часть помещения" },
            { GarAddressLevel.AutonomousRegionLevel, "Уровень автономного округа" },
            { GarAddressLevel.IntracityLevel, "Внутригородской уровень" },
            { GarAddressLevel.AdditionalTerritoriesLevel, "Уровень дополнительных территорий" },
            { GarAddressLevel.LevelOfObjectsInAdditionalTerritories, "Уровень объектов на дополнительных территориях" },
            { GarAddressLevel.CarPlace, "Машиноместо" }
        };

        public static string GetText(GarAddressLevel level) =>
            LevelTexts.TryGetValue(level, out var text) ? text : "Неизвестный уровень";


    }
}
