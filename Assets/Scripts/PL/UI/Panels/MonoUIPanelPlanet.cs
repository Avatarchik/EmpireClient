/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Панель отображения свойств планеты           }
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
    public class MonoUIPanelPlanet : MonoBehaviour
    {
        // Панель хранилища
        public MonoUIPanelStorage Storage;
        // Панель постройки корабликов
        public MonoUIPanelConstruction Construct;
        // Панель постройки строений
        public MonoUIPanelBuildings Buildings;
        // Панель элекричества
        public GameObject EnergyPanel;
        // Метка количества энергии
        public Text EnergyValue;
        // Кнопка закрытия
        public Button ButtonClose;
        // Метка описания владельца
        public Text Owner;

        // Планета, для которой открыто окно деталей
        private Planet FActivePlanet;

        void Start()
        {
            ButtonClose.onClick.AddListener(Close);
        }

        // Смена количества слотов для построек
        private void UpdateBuildings()
        {
            Buildings.ChangeData(FActivePlanet, FActivePlanet.Owner.Role, FActivePlanet.IsManned ? 12 : 8);
        }

        // Обновление слотов хранилища
        public void UpdateStorage(Planet APlanet, bool AClear)
        {
            if (FActivePlanet != APlanet)
                return;
            Storage.Resize(FActivePlanet.StorageCount, AClear);
        }

        // Обновление данных энергии
        public void UpdateEnergy(Planet APlanet)
        {
            if (FActivePlanet != APlanet)
                return;
            EnergyValue.text = FActivePlanet.EnergyCount.ToString();
        }

        // Обновление слотов постройки кораблей по обновленным данным
        public void UpdateConstruct(Planet APlanet)
        {
            if (FActivePlanet != APlanet)
                return;
            Construct.ChangeData(APlanet.ModulesCount, FActivePlanet.Owner.Role == SSHRole.Self, 
                FActivePlanet.ShipyardsCount > 0, FActivePlanet.IsManned);
        }

        // Обновление текстовок
        public void UpdateDescription()
        {
            Color LColor = Color.red;
            switch (FActivePlanet.Owner.Role)
            {
                case SSHRole.Self:
                    LColor = SSHLocale.IntToColor(0x00CAFFB2); break;
                case SSHRole.Friend:
                    LColor = Color.green; break;
                case SSHRole.Neutral:
                    LColor = SSHLocale.IntToColor(0xF0F0F0AE); break;
                case SSHRole.Enemy:
                    LColor = SSHLocale.IntToColor(0xFF8E0090); break;
            }

            if (FActivePlanet.IsManned)
                Owner.text = FActivePlanet.Class.ToString() + " <color=\"#" + ColorUtility.ToHtmlStringRGB(LColor) + "\">" + FActivePlanet.Name + "</color>";
            else
                Owner.text = "<color=\"#CBCC3C\">" + FActivePlanet.PlanetType.ToString() + "</color>";
        }

        // Запрос сервера на предмет отображения свойства планеты
        public void Show(Planet APlanet)
        {
            if (FActivePlanet != APlanet)
            {
                // Обновим набор строений
                FActivePlanet = APlanet;
                UpdateBuildings();
                Engine.SocketWriter.PlanetShowDetails(FActivePlanet.UID);
            }
            else Close();
        }

        // Ответ от сервера что все данные переданы и можно показать панель деталей
        public void Show()
        {
            // Энергия есть только у жилых планет
            EnergyPanel.SetActive(FActivePlanet.IsManned);
            // Обновим описание
            UpdateDescription();
            // Обнулим параметры построек
            Construct.Close();
            // Покажем панельку
            transform.gameObject.SetActive(true);
        }

        // Скрыть панельку деталей
        public void Close()
        {
            FActivePlanet = null;
            transform.gameObject.SetActive(false);
        }

        // Возврат объекта планеты, для которой открыта панель деталей
        public Planet ActivePlanet()
        {
            return FActivePlanet;
        }

        // Возврат номера планеты, для которой открыта панель деталей
        public int ActivePlanetId()
        {
            if (FActivePlanet != null)
                return FActivePlanet.UID;
            else
                return 0;
        }
    }
}