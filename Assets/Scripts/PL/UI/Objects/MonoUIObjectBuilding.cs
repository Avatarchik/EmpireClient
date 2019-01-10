/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления планетарным строением      }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using UnityEngine.UI;

namespace Planetar
{
    public class MonoUIObjectBuilding : MonoBehaviour
    {
        // Кнопка покупки
        public Button ButtonBuy;
        // Кнопка постройки
        public Button ButtonConstruct;
        // Значение прогресса постройки
        public Image ButtonConstructValue;
        // Картинка здания
        public Image Model;
        // Картинка фона
        public Image PlaceSprite;

        // Ссылка на объект здания
        private Transform FTransform;
        // Ссылка на панель технологий
        private MonoUIPanelTechBuildings FPanelTech;
        // Тип строения
        private BuildingType FType;
        // Уровень строения
        private int FLevel;
        // Номер слота
        private int FPosition;
        // Признак показа кнопки постройки
        private bool FIsConstruct;

        // Управление постройкой здания
        private void DoConstruct(int AHP)
        {
            // Если постройки небыло, а структура для постройки появилась - запуск анимации
            if ((!FIsConstruct) && (AHP > 0))
            {
                FIsConstruct = true;
                DoBuildingChange((FLevel + 1));
            }
            else
            {
                // Если все построили - выключаем анимацию
                if ((FIsConstruct) && (AHP == 0))
                    FIsConstruct = false;
            }
            // Анимация постройки, по 1000хп на уровень здания
            if (FIsConstruct)
            {
                int LMaxHP = 1000 * (FLevel + 1);
                ButtonConstructValue.fillAmount = (float)(LMaxHP - AHP) / LMaxHP;
            }
            ButtonConstruct.gameObject.SetActive(FIsConstruct);
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

        // Открытие панели технологий для покупки здания
        private void DoBuyBuildingByButton()
        {
            FPanelTech.Show(FPosition, FType, FLevel);
        }

        // Покупка предыдущего купленного типа здания
        private void DoBuyBuildingByHotKey()
        {
            Engine.SocketWriter.PlanetBuildingConstruct(Engine.UIPlanetDetails.ActivePlanetId(), FPosition, (int)FType);
        }

        // Создания контрола здания
        public void InitData(int APosition, MonoUIPanelTechBuildings APanelTech, BuildingType AType = BuildingType.Empty)
        {
            FTransform = transform;
            FPosition = APosition;
            FPanelTech = APanelTech;
            FType = AType;
            Model.enabled = false;
            ButtonBuy.onClick.AddListener(DoBuyBuildingByButton);
        }

        // Обновление параметров строения
        public void UpdateBuildingData(BuildingType AType, int ALevel, int AHP)
        {
            FType = AType;
            FLevel = ALevel;
            // Включение и выключение режима постройки
            DoConstruct(AHP);
            // Смена картинки, постройка сама задает картинку
            if (!FIsConstruct)
                DoBuildingChange(ALevel);
        }

        // Обновление параметров кнопки
        public void UpdateButtons(bool AActive)
        {
            ButtonBuy.interactable = AActive;
        }

        // Обновление фона строения
        public void UpdatePlace(Vector3 APosition, Sprite ASprite)
        {
            FTransform.localPosition = APosition;
            PlaceSprite.sprite = ASprite;
        }
    }
}