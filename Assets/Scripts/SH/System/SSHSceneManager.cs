using UnityEngine;

namespace Shared
{
    public class SSHSceneManager : MonoBehaviour
    {
        // Буффер чтения
        private SSHConnectionCommandBuffer FData;

        // Выгрузка принятых комманд
        protected void DoReadQueue(SSHSocketReader AReader)
        {
            // Обработаем сообщение из очереди
            while (SSHConnection.Dequeue(out FData))
                AReader.Read(FData.Command, FData.Buffer);
        }
    }
}