/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления снарядом типа ракета       }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2018.02.22                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    // Класс управления снарядом типа ракета
    public class MonoShipShellRocket : MonoShipShellCustom
    {
        // Картинка снаряда
        public Transform Model;
        // Картинка снаряда
        public MeshRenderer Mesh;
        // Цвет начала лазера союзных войск
        public Material ColorFriend;
        // Цвет начала лазера союзных войск
        public Material ColorEnemy;

        // Время перезарядки лазера
        private float FReloadTime = 2.0f;
        // Расстояние уничтожении пули
        private float FStopDistance = 0.1f;
        // Скорость полета пули
        private float FFlySpeed = 4f;

        // Подготовка снаряда к запуску
        protected override float DoPrepare()
        {
            if (FRole == SSHRole.Enemy)
                Mesh.material = ColorEnemy;
            else
                Mesh.material = ColorFriend;
            // Таймаут выстрела
            return FReloadTime;
        }

        // Расчет полета снаряда
        protected override void DoMoveToTarget()
        {
            // Повернем снаряд в сторону противника
            FTransform.LookAt(FTarget);
            // Сдвинем на дельту
            if (Vector3.Distance(FTarget.position, FTransform.position) >= FStopDistance)
                FTransform.position = Vector3.MoveTowards(FTransform.position, FTarget.position, FFlySpeed * Time.deltaTime);
            else
                Destroy(gameObject);
        }
    }
}