using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data
{
    public class DatabaseMigrationsProvider
    {
        public static void RunMigrations(AppDbContext context)
        {
            var canConnect = false;
            //int connectCount = 0;
            //do
            //{
                //canConnect = CheckDBConnection(context);
                //connectCount++;

                //if (connectCount == 3)
                //{
                //    throw new DatabaseConnectionException("Database connection failed. Kindly confirm the connection string");
                //}
            //} while (canConnect == false);

            var pendingMigrations = context.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                context.Database.Migrate();
            }
        }

        private static bool CheckDBConnection(AppDbContext context)
        {
            var canConnect = context.Database.CanConnect();

            if (canConnect)
            {
                return true;
            }

            return false;
        }
    }
}
