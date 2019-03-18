/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления орудием корабля            }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2018.02.22                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    // Класс управления орудием корабля
    public class ShipWeapon : Object
    {
        // Цель полета снаряда
        public Ship Ship;

        // Наш объект
        private Transform FTransform;
        // Роль относительн игрока
        private Player FOwner;
        // Время последнего действия
        private float FLastTime;
        // Тип снаряда
        private ShipShellType FShellType;
        // Время действия снаряда
        private float FShellTime;

        // Воспроизведение выстрела
        private void DoShot()
        {
            // Зафиксируем время последнего выстрела
            FLastTime = Time.time;
            // Создать снаряд и отправить ее на полет
            Transform LBullet = PrefabManager.CreateShell(FTransform, Vector3.zero, FShellType).transform;
            LBullet.SetParent(FTransform, false);
            FShellTime = LBullet.GetComponent<MonoShipShellCustom>().Init(FTransform, Ship.Transform, FOwner.Role);
        }

        // Проверка на возможность атаки
        public void Attack()
        {
            if ((Ship != null)
                && (Time.time > FLastTime + FShellTime * Random.Range(1f, 3f)))
                DoShot();
        }

        // Задание цели для атаки
        public void Retarget(Ship AShip)
        {
            Ship = AShip;
        }

        // Конструктор орудия
        public ShipWeapon(Transform AGunPosition, ShipShellType AShellType, Player AOwner)
        {
            FTransform = AGunPosition;
            FShellType = AShellType;
            FOwner = AOwner;
            FLastTime = int.MinValue;
        }
    }
}