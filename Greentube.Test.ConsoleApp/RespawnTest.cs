using Respawn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Greentube.Test.ConsoleApp
{
    public class RespawnTest
    {
        private Checkpoint _checkPoint { get; set; }

        public void Init()
        {
            _checkPoint = new Checkpoint();
            _checkPoint.DbAdapter = DbAdapter.SqlServer;
        }

        public void Reset()
        {
            _checkPoint.Reset("AdminDbConnection");
        }

        public void DoOneMinuteTestThanReSpawn()
        {
            Init();

            for (var ii = 0; ii < 10; ii++)
            {
                Console.WriteLine(ii + 1);
                Thread.Sleep(10000);
            }
        }
    }
}