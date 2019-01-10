/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль кнопки покупки строения               }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoUIObjectBuildingBuy : MonoBehaviour
    {
        // Метка названия строения
        public Text Name;
        // Кнопка покупки
        public Button ButtonBuy;
        // Кнопка текста покупки
        public Text ButtonBuyText;
        // Кнопка апгрейда
        public Button ButtonUpgrade;
        // Иконка необходимости апгрейда
        public Transform BuyLock;
        // Картинка здания
        public Image Model;

        // Ссылка на объект здания
        private Transform FTransform;
        // Тип строения
        private BuildingType FType;
        // Уровень строения
        private int FLevel;
        // Номер слота
        private int FPosition;
        // Признак показа кнопки покупки
        private bool FIsShowBuy;
        // Признак наличия строения
        private bool FIsActive;

        // Смена состояния контролам
        private void DoUpdateButtons()
        {
            // Кнопка покупки доступна всегда, если разрешена
            ButtonBuy.gameObject.SetActive(FIsShowBuy);
            // Подпись к кнопке покупки только на панели технологий
            ButtonBuyText.gameObject.SetActive(FIsShowBuy);
            // Кнопка апгрейда показывается если уровень купелнной технологии не максимальный
            ButtonUpgrade.gameObject.SetActive(FIsActive 
                && (Engine.TechBuilding(FType,BuildingTech.Active).Level < Engine.TechBuilding(FType, BuildingTech.Active).Count));
            // Кнопка блока покупки показывается если для покупки не хватает технологии
            BuyLock.gameObject.SetActive(!FIsShowBuy && ButtonUpgrade.gameObject.activeSelf);
        }

        // Смена состояния возможности покупки здания
        private void DoUpdateButtonsTech(BuildingType AType, int ALevel)
        {
            int LLevel = Engine.TechBuilding(FType, BuildingTech.Active).Level;
            FIsShowBuy = ((AType == BuildingType.Empty) && (LLevel > 0))
                || ((AType == FType) && (LLevel > ALevel));
            FIsActive = true;
            // Обновление кнопок
            DoUpdateButtons();
            // Обновление картинки здания
            if (LLevel == 0) LLevel++;
            DoBuildingChange(LLevel);
        }

        // Смена картинки строения
        private void DoBuildingChange(int ALevel)
        {
            if (FType == BuildingType.Empty)
                Model.enabled = false;
            else
            {
                Model.sprite = SpriteManager.Building(FType, ALevel);
                Model.enabled = true;
            }
        }

        // Покупка здания
        private void DoBuyBuilding()
        {
            Engine.SocketWriter.PlanetBuildingConstruct(Engine.UIPlanetDetails.ActivePlanetId(), FPosition, (int)FType);
            Engine.UIPlanetDetails.Buildings.PanelTech.Close();
        }

        // Покупка технологии
        private void DoBuyTech()
        {
            Engine.SocketWriter.PlanetBuildingUpgrade((int)FType);
        }

        // Создания контрола здания
        public void InitData(int APosition, BuildingType AType = BuildingType.Empty)
        {
            FPosition = APosition;
            FType = AType;
            // Первичная настройка параметров
            if (FTransform)
                return;
            FTransform = transform;
            ButtonBuy.onClick.AddListener(DoBuyBuilding);
            ButtonUpgrade.onClick.AddListener(DoBuyTech);
            Model.enabled = false;
            Name.text = Engine.InfoBuildings[(int)AType].Name;
        }

        // Обновление параметров технологии
        public void UpdateBuildingTech(BuildingType AType, int ALevel, int APosition)
        {
            FPosition = APosition;
            DoUpdateButtonsTech(AType, ALevel);
        }
    }
}