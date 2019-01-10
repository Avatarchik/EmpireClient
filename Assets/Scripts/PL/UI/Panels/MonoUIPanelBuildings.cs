/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления панелью строений           }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;

namespace Planetar
{
    public class MonoUIPanelBuildings : MonoBehaviour
    {
        // Панель технологий
        public MonoUIPanelTechBuildings PanelTech;
        // Количество слотов на большой планете
        private const int C_SLOT_COUNT_BIG = 12;
        // Количество слотов на маленькой планете
        private const int C_SLOT_COUNT_SMALL = 8;
        // Позиции площадок для большой планеты
        private Vector3[] FCoordBy10 = new Vector3[] { 
            new Vector3(66,  -81,  0),
            new Vector3(123, -46,  0),
            new Vector3(180, -81,  0),
            new Vector3(237, -46,  0),
            new Vector3(294, -81,  0),
            new Vector3(66,  -149, 0),
            new Vector3(123, -184, 0),
            new Vector3(180, -149, 0),
            new Vector3(237, -184, 0),
            new Vector3(294, -149, 0),
            new Vector3(123, -115, 0),
            new Vector3(240, -115, 0)
        };
        // Позиции площадок для маленькой планеты
        private Vector3[] FCoordBy7 = new Vector3[] { 
            new Vector3(66,  -81,  0),
            new Vector3(123, -46,  0),
            new Vector3(237, -46,  0),
            new Vector3(294, -81,  0),
            new Vector3(123, -115, 0),
            new Vector3(240, -115, 0),
            new Vector3(180, -149, 0),
            new Vector3(180, -81,  0)
        };
        // Кэш панели
        private Transform FTransform;
        // Список строений
        private List<MonoUIObjectBuilding> FBuildings;
        // Планета
        private Planet FPlanet;

        // Создание экземпляра строения
        void DoBuildingsCreate(int ACount, bool AChange, bool AEnabled)
        {
            Transform LBuildingSlot;
            MonoUIObjectBuilding LBuilding;
            // Заготовка создается один раз
            if (FBuildings.Count == 0)
            {
                for (int LIndex = 0; LIndex < C_SLOT_COUNT_BIG; LIndex++)
                {
                    LBuildingSlot = PrefabManager.CreateBuildingSlot(FTransform, Vector3.zero).transform;
                    LBuildingSlot.SetParent(FTransform, false);
                    LBuilding = LBuildingSlot.GetComponent<MonoUIObjectBuilding>();
                    LBuilding.InitData(LIndex, PanelTech);
                    FBuildings.Add(LBuilding);
                }
            }
            // Далее нужно включить нужные слоты
            int LCount;
            int LActiveCount;
            // В больших 2 пустых, в маленьких 1 пустой, в остальных слотов нет
            if (FPlanet.PlanetType == PlanetType.Big)
            {
                LCount = C_SLOT_COUNT_BIG;
                LActiveCount = LCount - 2;
            }
            else
                if (FPlanet.PlanetType == PlanetType.Small)
                {
                    LCount = C_SLOT_COUNT_SMALL;
                    LActiveCount = LCount - 1;
                }
                else
                {
                    LCount = 0;
                    LActiveCount = 0;
                }
            // Теперь переберем все слоты, включим нужные и зададим им координаты
            Vector3 LPosition;
            for (int LIndex = 0; LIndex < C_SLOT_COUNT_BIG; LIndex++)
            {
                bool LNonBuild = (LIndex >= LActiveCount);
                // Меняем только те здания, которые показываются, скрытые не чистим
                if (LIndex < LCount)
                {
                    if ((FPlanet.PlanetType == PlanetType.Small) && (LIndex < C_SLOT_COUNT_SMALL))
                        LPosition = FCoordBy7[LIndex];
                    else
                        LPosition = FCoordBy10[LIndex];
                    // Меняем состояние для кнопок всех строений
                    FBuildings[LIndex].UpdateButtons(AEnabled && !LNonBuild);
                    // При смене планеты меняем фон
                    if (AChange)
                    {
                        FBuildings[LIndex].UpdateBuildingData(BuildingType.Empty, 0, 0);
                        FBuildings[LIndex].UpdatePlace(LPosition, SpriteManager.BuildingPlace(FPlanet.PlanetType, !LNonBuild));
                    }
                }
                // Скрытые скрываем
                FBuildings[LIndex].gameObject.SetActive(LIndex < LCount);
            }
        }

        // Смена параметров строения
        public void BuildingUpdate(int APlanet, int AIndex, BuildingType AType, int ALevel, int AHP)
        {
            if (Engine.UIPlanetDetails.ActivePlanetId() != APlanet)
                return;
            FBuildings[AIndex].UpdateBuildingData(AType, ALevel, AHP);
        }

        // Смена параметров построек
        public void ChangeData(Planet APlanet, SSHRole ARole, int ACount)
        {
            if (FTransform == null)
            {
                FTransform = transform;
                FBuildings = new List<MonoUIObjectBuilding>();
            }
            bool LChange = (FPlanet != APlanet);
            FPlanet = APlanet;
            DoBuildingsCreate(ACount, LChange, ARole == SSHRole.Self);
        }
    }
}