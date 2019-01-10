/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления снарядом типа пуля         }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2018.02.22                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    // Класс управления снарядом типа пуля
    public class MonoShipShellBullet : MonoShipShellCustom
    {
        // Картинка снаряда
        public SpriteRenderer Bullet;
        // Цвет начала лазера союзных войск
        public Sprite ColorFriend;
        // Цвет начала лазера союзных войск
        public Sprite ColorEnemy;

        // Время перезарядки лазера
        private float FReloadTime = 0.4f;
        // Расстояние уничтожении пули
        private float FStopDistance = 0.1f;
        // Скорость полета пули
        private float FFlySpeed = 2f;

        // Подготовка снаряда к запуску
        protected override float DoPrepare()
        {
            if (FRole == SSHRole.Enemy)
                Bullet.sprite = ColorEnemy;
            else
                Bullet.sprite = ColorFriend;
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