/*///////////////////////////////////////////////
{                                              }
{ Planetar                                     }
{ Модуль управления группами корабликов        }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2017.02.05                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using System.Collections.Generic;

namespace Planetar
{
    public class ShipGroup : Shared.Interactive
    {
        // Список кораблей в группе
        public List<Ship> Ships;
        // Список планет в пути
        public List<Planet> Planets;
        // Последняя обработанная планета
        public Planet LastPlanet;

        // Конструктор
        public ShipGroup()
        {
            Ships = new List<Ship>();
            Planets = new List<Planet>();
        }

        // Группа автоматически инициализируется, если есть кораблики
        private void DoSetGroup()
        {
            /*if (Ships.Count > 0)
                SSHShared.ObjectSource = Shared.ShipGroup;
            else
                SSHShared.ObjectSource = null;*/
        }

        // Добавление планеты в путь полета
        private void DoAddPath(Planet APlanet)
        {
            Planets.Add(APlanet);

            foreach (Ship LShip in Ships)
                LShip.PathAdd(APlanet);

            LastPlanet = APlanet;
        }

        // Поиск оптимального пути методом Ли
        private void DoFindPathForward(Planet AStart, Planet AFinish)
        {
            // Поиск пути с последней используемой до указанной планеты
            foreach (Planet LPlanet in Engine.MapPlanets)
                LPlanet.Weight = float.MaxValue;
            AStart.Weight = 1;
            // Очередь планет для обработки, начинаем со стартового сектора
            Queue<Planet> LPlanets = new Queue<Planet>();
            LPlanets.Enqueue(LastPlanet);
            // Очередь всегда кончается для полного созвездия
            while (LPlanets.Count > 0)
            {
                Planet LSource = LPlanets.Dequeue();
                // По заранее определенным линкам немного быстрее
                foreach (Planet LPlanet in LSource.Links)
                {
                    if (LPlanet.PlanetType == PlanetType.Hole)
                        continue;
                    if (LPlanet.Weight == float.MaxValue)
                        LPlanet.Weight = 0;
                    else
                        if (LPlanet.Weight != 0)
                            continue;
                    LPlanet.Weight = LSource.Weight + 1;
                    if (LPlanet == AFinish)
                    {
                        // Поиск оптимального пути из найденных
                        DoFindPathBack(AFinish, AStart);
                        return;
                    }
                    LPlanets.Enqueue(LPlanet);
                }
            }
        }

        // Построение пути между планетами в указанном пути секторов
        private void DoFindPathBack(Planet AStart, Planet AFinish)
        {
            Stack<Planet> LPlanets = new Stack<Planet>();
            LPlanets.Push(AStart);
            // Определение наикратчайшего пути по секторам
            while (AStart != AFinish)
            {
                float LMinWeight = float.MaxValue;
                foreach (Planet LPlanet in AStart.Links)
                {
                    if (LPlanet.Weight < LMinWeight)
                    {
                        AStart = LPlanet;
                        LMinWeight = LPlanet.Weight;
                    }
                }
                if (AStart != AFinish)
                    LPlanets.Push(AStart);
            }
            // Переберем массив с конца для создания пути
            while (LPlanets.Count > 0)
                DoAddPath(LPlanets.Pop());
        }

        // Очистка группы
        public void Clear()
        {
            foreach (Ship AShip in Ships)
                AShip.ShowPath(false);
            LastPlanet = null;
            Ships.Clear();
            Planets.Clear();
            DoSetGroup();
        }

        // Запуск полета группы кораблей
        public void Run(Planet APlanet)
        {
            if (LastPlanet != APlanet)
                Engine.ShipGroup.AddPath(APlanet);
            Engine.SocketWriter.ShipMoveToGroup(this);
            Clear();
        }

        // Добавление корабля в группу
        public void AddShip(Ship AShip)
        {
            if (LastPlanet == null)
                LastPlanet = AShip.Planet;
            Ships.Add(AShip);
            AShip.ShowPath(true);
            DoSetGroup();
        }

        // Удаление корабля из группы
        public void RemoveShip(Ship AShip)
        {
            Ships.Remove(AShip);
            AShip.ShowPath(false);
            DoSetGroup();
        }

        // Добавление пути следования
        public void AddPath(Planet APlanet)
        {
            // Если планета соседняя - сразу проставляем путь
            if (APlanet.Links.Contains(LastPlanet))
            {
                DoAddPath(APlanet);
                return;
            }
            // Если планета не соседняя - нужно найти оптимальный путь
            else
                DoFindPathForward(LastPlanet, APlanet);
        }

        // Удаление последней планеты пути следования
        public void RemovePath(Planet APlanet)
        {
            Planets.Remove(APlanet);

            foreach (Ship LShip in Ships)
                LShip.PathRemove();
        }
    }
}