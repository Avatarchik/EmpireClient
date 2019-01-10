/*///////////////////////////////////////////////
{                                              }
{ Galaxy                                       }
{ Модуль создания префабов                     }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev A  2017.01.08                            }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using UnityEngine;

namespace Galaxy
{
    public class SGAPrefabManager : Shared.PrefabManager
    {
        // Создание префаба кластера
        public static GameObject CreateCluster(Transform AParent, Vector3 APosition)
        {
            const string LPrefabResName = "GA/Cluster/pfGACluster";
            // И создадим 
            return Create(LPrefabResName, APosition);
        }
    }
}