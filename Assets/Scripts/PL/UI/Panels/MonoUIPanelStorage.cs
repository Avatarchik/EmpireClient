/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль обработки хранилища ресурсов          }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2016.12.18                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;
using System.Collections.Generic;

namespace Planetar
{
    public class MonoUIPanelStorage : MonoBehaviour
    {
        public Transform UIStorageSlots;

        // Признак, что зранилище не относится к планете
        public bool PlayerStorage;
        // Картинка активного слота
        public Sprite ActiveSlot;
        // Картинка неактивного слота
        public Sprite PassiveSlot;

        // Кэш самого себя
        private Transform FTransform;
        // Массив слотов, доступных для манипуляции
        private List<TSHClassStorageSlot> FSlots;
        // Количество слотов по умолчанию
        private int FStorageCount = 0;
        // Максимум слотов для планеты
        private const int FPlanetarStorageCount = 16;

        // Удаление объекта ресурса
        private void DoResourceClear(ref Shared.Resource AResource)
        {
            if (AResource != null)
            {
                AResource.HideHint();
                AResource.Slot.FadeOut();
                Destroy(AResource.Transform.gameObject);
                AResource = null;
            }
        }

        // Клонирование указанного количества слотов
        private void DoStorageCreate(int AStart, int ACount, bool AClear)
        {
            // Слоты всегда создаются для игрока, либо разово для планеты
            if ((PlayerStorage)
                || (FSlots.Count == 0))
            {
                TSHClassStorageSlot LSlot;
                int LDefCount;

                if (PlayerStorage)
                    LDefCount = ACount;
                else
                    LDefCount = FPlanetarStorageCount;
                for (int LIndex = AStart; LIndex < LDefCount; LIndex++)
                {
                    LSlot = new TSHClassStorageSlot(UIStorageSlots, LIndex + 1, PlayerStorage);
                    FSlots.Add(LSlot);
                }
            }
            // Для планеты включим нужные слоты, удалив данные о старых
            if (!PlayerStorage)
            {
                for (int LIndex = 0; LIndex < FPlanetarStorageCount; LIndex++)
                {
                    if (AClear)
                    {
                        // Удалить висящую модельку
                        DoResourceClear(ref FSlots[LIndex].Resource);
                        // Выключить все тексты
                        FSlots[LIndex].ChangeEnable(false);
                    }
                    // Включаем только нужные слоты и делаем их активными, другие выключаем
                    FSlots[LIndex].ChangeState(LIndex < ACount, ActiveSlot);
                }
            }
            FStorageCount = ACount;
        }

        // Событие открытия контекстного меню на слоте хранилища
        private void DoSlotContextMenu(Shared.Interactive AStorage)
        {
            Debug.Log("alarma " + AStorage.ToString());
        }

        // Логика перемещения ресурса
        private void DoResourceMove(Shared.Resource AResFrom, bool AHotkey)
        {
            /*int LTargetPlanet = -1;
            int LSourcePlanet = -1;
            int LTargetSlot = 0;
            bool LOnePlace = false;
            Planet LPlanet;*/
            // Ручной перенос без хоткея
            if (!AHotkey)
            {
                //TSHClassStorageSlot LSlotTo = null;
                //TSHClassResource LResTo = null;
                // Дополнение или обмен ресурсов
                /*if (Interactive.IsResource(SSHShared.MonoTarget, out LResTo))
                {
                    LOnePlace = (LResTo.Slot.PlayerStorage == AResFrom.Slot.PlayerStorage);
                    LTargetSlot = LResTo.Slot.UID;
                }
                // Перемещение между хранилищами
                else if (Interactive.IsStorage(SSHShared.MonoTarget, out LSlotTo))
                {
                    LOnePlace = (LSlotTo.PlayerStorage == AResFrom.Slot.PlayerStorage);
                    LTargetSlot = LSlotTo.UID;
                }*/
            }
            // Дроп на планету с хранилища
            /*if (Interactive.IsPlanet(Shared.ObjectTarget, out LPlanet))
            {
                LOnePlace = false;
                LTargetPlanet = LPlanet.UID;
            }
            // Из хранилища на планету
            else if (AResFrom.Slot.PlayerStorage)
            {
                if (!LOnePlace)
                    LTargetPlanet = Shared.UIPlanetDetails.ActivePlanetId();
                else
                    LTargetPlanet = LSourcePlanet;
            }
            // Из планеты в хранилище
            else
            {
                LSourcePlanet = Shared.UIPlanetDetails.ActivePlanetId();
                if (LOnePlace)
                    LTargetPlanet = LSourcePlanet;
            }
            // Проверим что есть смысл отправлять
            if ((LTargetSlot == AResFrom.Slot.UID) && (LTargetPlanet == LSourcePlanet))
                return;
            // Отправим запрос на перемещение
            Shared.SocketWriter.SendResourceMove(LSourcePlanet, AResFrom.Slot.UID,
                LTargetPlanet, LTargetSlot, AResFrom.Count, LOnePlace);*/
        }

        // Логика обновления любого хранилища по запросу сервера
        private void DoStorageUpdate(TSHClassStorageSlot ASlot, Shared.ResourceType AResourceType, int ACount)
        {
            // Удалим старый ресурс, если добавляется ресурс другого типа
            if ((ASlot.Resource != null) && (ASlot.Resource.ResType != AResourceType))
                DoResourceClear(ref ASlot.Resource);
            // Если входящий ресурс есть
            if (AResourceType != Shared.ResourceType.Empty)
            {
                // Создадим ресурс, если его нет в слоте
                if (ASlot.Resource == null)
                {
                    ASlot.Resource = new Shared.Resource(ASlot.Transform, AResourceType, ASlot);
                    ASlot.Resource.OnResourceMove = DoResourceMove;
                }
                ASlot.Resource.Count = ACount;
            }
            // Обновим количество ресурса
            ASlot.ChangeCount(ACount);
            ASlot.ChangeEnable(ACount > 0);
        }

        // Очистка слота от ресурсов
        public void Clear(int APlanet, int AIndex)
        {
            if (Engine.UIPlanetDetails.ActivePlanetId() != APlanet)
                return;
            TSHClassStorageSlot LSlot = FSlots[AIndex - 1];
            // Удалим сам ресурс если он есть
            DoResourceClear(ref LSlot.Resource);

            // Скроем слот если он был неактивным или цифры если активный
            if (!LSlot.Active)
                LSlot.ChangeState(false, null);
            else
                LSlot.ChangeEnable(false);
        }

        // Обновление данных пользовательского слота
        public void Change(int ASlot, int AResID, int ACount)
        {
            // Обновим сам слот
            DoStorageUpdate(FSlots[ASlot - 1], (Shared.ResourceType)AResID, ACount);
        }

        // Обновление данных указанного слота, создание ресурсов в нем
        public void Change(int APlanet, int AIndex, int AResID, int ACount, int AFlags, bool AActive)
        {
            if (Engine.UIPlanetDetails.ActivePlanetId() != APlanet)
                return;
            TSHClassStorageSlot LSlot = FSlots[AIndex - 1];
            // Выставление фона слоту
            if (!AActive)
                LSlot.ChangeState(true, PassiveSlot);
            else
                LSlot.ChangeState(true, ActiveSlot);
            // Обновим сам слот
            LSlot.Active = AActive;
            DoStorageUpdate(LSlot, (Shared.ResourceType)AResID, ACount);
        }

        // Внешний метод обновления сведений
        public void Resize(int ACount, bool AClear)
        {
            if (FTransform == null)
            {
                FTransform = transform;
                FSlots = new List<TSHClassStorageSlot>();
            }
            DoStorageCreate(FStorageCount, ACount, AClear);
        }
    }
}