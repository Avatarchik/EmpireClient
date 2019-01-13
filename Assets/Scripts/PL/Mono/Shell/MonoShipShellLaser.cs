/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления снарядом типа лазер        }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2018.02.22                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    // Класс управления снарядом типа лазер
    public class MonoShipShellLaser : MonoShipShellCustom
    {
        // Рендер лазера
        public LineRenderer Line;
        // Цвет начала лазера союзных войск
        public Material ColorFriend;
        // Цвет начала лазера союзных войск
        public Material ColorEnemy;

        // Время жизни лазера
        private float FReloadTime = 4;
        // Время жизни лазера
        private float FLifeTime = 2;
        // Время появления лазера
        private float FAlphaTime = 0.3f;

        // Подготовка снаряда к запуску
        protected override float DoPrepare()
        {
            // Цвет лазера
            if (FRole == SSHRole.Enemy)
                Line.material = ColorEnemy;
            else
                Line.material = ColorFriend;
            Line.enabled = true;
            // Таймаут выстрела
            return FReloadTime;
        }

        // Расчет позиции лазера
        protected override void DoMoveToTarget()
        {
            Line.SetPosition(0, FSource.position);
            // Процент времени стрельбы
            float LPercent = (Time.time - FTime) / FLifeTime;
            // Прозрачность лазера
            float LAlphaColor;
            // Определим уровень прозрачности, 0..2 и 8..10 для плавного появления
            if (LPercent < FAlphaTime)
                LAlphaColor = LPercent / FAlphaTime;
            else if (LPercent > (1 - FAlphaTime))
                LAlphaColor = (1 - LPercent) / FAlphaTime;
            else LAlphaColor = 1;
            // Выставим цвет
            Color LLineColor = Line.startColor;
            LLineColor.a = LAlphaColor;
            Line.startColor = LLineColor;
            // Вычислим перемещение лазера по поверхности противника
            Vector3 LPosition = Vector3.MoveTowards(FTarget.position, Vector3.down, (LPercent / 4) - 0.15f);
            Line.SetPosition(1, LPosition);
            // Удаление объекта по завершению времени
            if (LPercent >= 1f)
                Destroy(gameObject);
        }
    }
}