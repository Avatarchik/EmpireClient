/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Внешний класс ячейки координатной сетки      }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    public class Grid
    {
        /* магическая константа */
        // Множитель шага сетки
        private float C_CAMERA_ORIGIN = 3.4f * 2;

        // Конструктор сразу определяет готовый префаб
        public Grid(Transform AParent, int ACoordX, int ACoordY)
        {
            Transform LSlot = PrefabManager.CreatePlaneGrid(ACoordX * C_CAMERA_ORIGIN, -ACoordY * C_CAMERA_ORIGIN).transform;
            LSlot.SetParent(AParent, false);
            LSlot.transform.Find("Coords").GetComponent<TextMesh>().text = ACoordX + "," + ACoordY;
        }
    }
}