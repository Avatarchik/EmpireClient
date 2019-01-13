/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Панель горячих кнопок для навигации          }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    public class MonoUIPanelNavigation : MonoBehaviour
    {
        // Показать технологии корабликов
        public void ShowShipTech()
        {
            Engine.SceneManager.PanelTechShips.Show();
        }

        // Выйти на галактическую карту
        public void ShowGalaxy()
        {
            Engine.SocketWriter.SendSelectGalaxy();
        }

        public void ShowExit()
        {
            SSHShared.ShowWelcome(true);
        }

        public void Close()
        {
            Application.Quit();
        }
    }
}