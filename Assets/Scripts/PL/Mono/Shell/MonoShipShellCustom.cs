/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления базового снаряда           }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2018.02.22                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Planetar
{
    // Базовый класс снаряда орудия
    public abstract class MonoShipShellCustom : MonoBehaviour
    {
        // Наш объект
        protected Transform FTransform;
        // Источник полета снаряда
        protected Transform FSource;
        // Цель полета снаряда
        protected Transform FTarget;
        // Роль пульки
        protected SSHRole FRole;
        // Время выстрела
        protected float FTime;

        // Позиция самого орудия
        private void Start()
        {
            FTransform = transform;
        }

        // Обновление полета снаряда
        private void Update()
        {
            if (FTarget != null)
                DoMoveToTarget();
            else
                DoDestroy();
        }

        // Уничтожение снаряда пока без эффектов
        private void DoDestroy()
        {
            Destroy(FTransform.gameObject);
        }

        // Подготовка снаряда к запуску
        protected abstract float DoPrepare();

        // Полет снаряда до цели, для каждого снаряда должна быть своя логика
        protected abstract void DoMoveToTarget();

        // Настройка снаряда
        public float Init(Transform ASource, Transform ATarget, SSHRole ARole)
        {
            FSource = ASource;
            FTarget = ATarget;
            FRole = ARole;
            FTime = Time.time;
            // Подготовим снаряд
            return DoPrepare();
        }
    }
}