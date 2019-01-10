/*///////////////////////////////////////////////
{                                              }
{ Galaxy                                       }
{ Модуль чтения данных с сервера               }
{ Copyright (c) 2016 UAshota                   }
{                                              }
{ Rev B  2017.06.06                            }
{                                              }
/*///////////////////////////////////////////////
using System.IO;

namespace Galaxy
{
    public class SGASocketReader : SSHSocketReader
    {
        // Чтение буфера комманд
        protected override void DoRead(int ACommand, MemoryStream AReader)
        {
            base.DoRead(ACommand, AReader);
        }
    }
}