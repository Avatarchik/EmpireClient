/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Внешний класс планетарного строения          }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////

namespace Planetar
{
    // Типы строений
    public enum BuildingType
    {
        Electro,                // Электростанция
        Warehouse,              // Склад
        Modules,                // Модульная лаборатория
        Xenon,                  // Ксеноновая лаборатория
        Titan,                  // Титановая лаборатория
        Kremniy,                // Кремниевая лаборатория
        Crystal,                // Кристальная лаборатория
        CombinatFarm,           // Продовольственный комбинат
        CombinatElectro,        // Электротехнический комбинат
        CombinatCrystal,        // Антипротонный комбинат
        CombinatXenon,          // Плазмотронный комбинат
        MakeCredits,            // Поселение
        Makekvark,              // Кварковый завод
        Makeppl,                // Институт инженеров
        Makemodules,            // Модульная фабрика 
        Makefuel,               // Топливная фабрика
        Maketech,               // Институт механиков
        Makemines,              // Завод мин
        Makedrules,             // Завод ремонтных дроидов
        Makearmor,              // Завод бронемодулей
        Makedamage,             // Завод модулей урона
        Empty
    }

    // Технологии для строений
    public enum BuildingTech
    {
        BuyCost,                // Стоимость покупки строения
        UpgradeCost,            // Стоимость покупки уровня строения
        Active,                 // Признак доступности строения
        Energy,                 // Производство и потребление энергии
        Capture,                // Добавляемая лояльность за строение
        ResInCount11,           // Количество ресурса 1 на 1 линию производства
        ResInCount21,           // Количество ресурса 2 на 1 линию производства
        ResInCount12,           // Количество ресурса 1 на 2 линию производства
        ResInCount22,           // Количество ресурса 2 на 2 линию производства
        ResOutCount1,           // Количество производимого ресурса на 1 линии производства
        ResOutCount2,           // Количество производимого ресурса на 2 линии производства
        Empty
    }

    // Описание деталей строения
    public struct BuildingInfo
    {
        // Количество линий производства
        public const int C_WORK_COUNT = 2;
        // Тип строения
        public BuildingType BuildingType;
        // Название
        public string Name;
        // Описание
        public string Description;
        // Тип N используемого ресурса M производства
        public Shared.ResourceType[,] ResInType;
        // Тип выходного ресурса M производства
        public Shared.ResourceType[] ResOutType;
        // Количество выходного ресурса M производства
        public int[] ResOutCount;
    }
}