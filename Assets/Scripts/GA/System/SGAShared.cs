/*///////////////////////////////////////////////
{                                              }
{ Galaxy                                       }
{ Модуль глобальных переменных                 }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////

namespace Galaxy
{
    public class SGAShared : SSHShared
    {
        // Сокет чтения
        public static SGASocketReader SocketReader;
        // Сокет записи
        public static SGASocketWriter SocketWriter;

        // При инициализации сразу создадим все объекты
        static SGAShared()
        {
            SocketReader = new SGASocketReader();
            SocketWriter = new SGASocketWriter();
        }
    }
}