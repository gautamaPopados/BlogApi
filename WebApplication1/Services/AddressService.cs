using Microsoft.EntityFrameworkCore;
using WebApplication1.AuthentificationServices;
using WebApplication1.Data;
using WebApplication1.Data.DTO;
using WebApplication1.Data.Entities;
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
            var houseAddressses = new List<SearchAddressModel>();

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
                        objectLevel = (GarAddressLevel)(Int32.Parse(a.level)),
                        objectLevelText = GetAddressText((GarAddressLevel)(Int32.Parse(a.level) - 1))
                    })
                    .ToListAsync();

                var childHouses = _db.Houses.Where(h => childObjectIds.Contains(h.objectid));

                houseAddressses = await childHouses
                    .Select(h => new SearchAddressModel
                    {
                        objectId = h.objectid,
                        objectGuid = h.objectguid,
                        text = BuildHouseName(h),
                        objectLevel = GarAddressLevel.Building,
                        objectLevelText = GetAddressText(GarAddressLevel.Building)
                    })
                    .ToListAsync();
            }
            addressses.AddRange(houseAddressses);
            return addressses;
        }

        public async Task<List<SearchAddressModel>> Chain(Guid objectGuid)
        {
            var addressses = new List<SearchAddressModel>();
            var childHouse = await _db.Houses.FirstOrDefaultAsync(h => h.objectguid == objectGuid);

            if (childHouse != null)
            {
                var parentId = _db.Hierarchies.FirstOrDefault(h => h.objectid == childHouse.objectid).parentobjid;

                if (parentId != null)
                {
                    var parentGuid = _db.AddrElements.FirstOrDefault(a => a.objectid == parentId).objectguid;
                    objectGuid = parentGuid;
                }

                addressses.Add(new SearchAddressModel
                {
                    objectId = childHouse.objectid,
                    objectGuid = childHouse.objectguid,
                    text = BuildHouseName(childHouse),
                    objectLevel = GarAddressLevel.Building,
                    objectLevelText = GetAddressText(GarAddressLevel.Building)
                });
            }

            var address = await _db.AddrElements.FirstOrDefaultAsync(a => a.objectguid == objectGuid);

            if(address != null)
            {
                var newAddressModel = new SearchAddressModel()
                {
                    objectId = address.objectid,
                    objectGuid = address.objectguid,
                    text = address.typename + " " + address.name,
                    objectLevel = (GarAddressLevel)(Int32.Parse(address.level)),
                    objectLevelText = GetAddressText((GarAddressLevel)(Int32.Parse(address.level) - 1))
                };

                while (newAddressModel != null) 
                {
                    addressses.Add(newAddressModel);

                    var currentHierarchy = await _db.Hierarchies.Where(h => h.objectid == newAddressModel.objectId).FirstOrDefaultAsync();

                    if (currentHierarchy == null || currentHierarchy.parentobjid == null)
                    {
                        return null;
                    }

                    address = await _db.AddrElements.Where(a => a.objectid == currentHierarchy.parentobjid).FirstOrDefaultAsync();

                    if (address != null)
                    {
                        newAddressModel = new SearchAddressModel()
                        {
                            objectId = address.objectid,
                            objectGuid = address.objectguid,
                            text = address.typename + " " + address.name,
                            objectLevel = (GarAddressLevel)(Int32.Parse(address.level)),
                            objectLevelText = GetAddressText((GarAddressLevel)(Int32.Parse(address.level) - 1))
                        };

                        addressses.Add(newAddressModel);
                    }
                    else
                    {
                        newAddressModel = null;
                    }
                }
            }

            addressses.Reverse();

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
        private static readonly Dictionary<int?, string> HouseTexts = new()
        {
            { 1, "к." },
            { 2, "стр." },
            { 3, "соор." },
            { 4, "лит." }
        };

        public static string GetAddressText(GarAddressLevel level) =>
            LevelTexts.TryGetValue(level, out var text) ? text : "Неизвестный уровень";
        public static string GetHouseText(int? level) =>
            HouseTexts.TryGetValue(level, out var text) ? text : "Неизвестный уровень";

        public static string BuildHouseName(House house)
        {
            var houseName = "";

            if (!String.IsNullOrEmpty(house.housenum))
            {
                houseName += house.housenum;
            }
            if (house.addtype1.HasValue)
            {
                houseName +=  " " + GetHouseText(house.addtype1) + " "+ house.addnum1;
            }
            if (house.addtype2.HasValue)
            {
                houseName += " " + GetHouseText(house.addtype2) + " " + house.addnum2;
            }

            return houseName;
        }
    }
}
