namespace IronFoundry.ServiceBroker.Models
{
    public static class MsSqlTemplates
    {
        public static string CreateDatabase = @"DECLARE @res INT;
                                              IF DB_ID('{0}') IS NULL 
                                              BEGIN
                                                CREATE DATABASE [{0}];
                                                SET @res = 0;
                                              END
                                              ELSE SET @res = -1
                                              
                                              SELECT @res";
        public static string CreateLimitedDatabase = @"DECLARE @res INT;
                                              IF DB_ID('{0}') IS NULL 
                                              BEGIN
                                                CREATE DATABASE [{0}] CONTAINMENT=PARTIAL ON PRIMARY (MAXSIZE = {2}MB, FILENAME='{1}{0}.mdf', Name='{0}');
                                                SET @res = 0;
                                              END
                                              ELSE SET @res = -1
                                              
                                              SELECT @res";

        public static string DropDatabase = @"DECLARE @res INT;
                                            IF DB_ID('{0}') IS NOT NULL 
                                           
                                            BEGIN
                                              ALTER DATABASE [{0}]
                                              SET SINGLE_USER
                                              WITH ROLLBACK IMMEDIATE;
                                              DROP DATABASE [{0}] 
                                              SET @res = 0
                                            END
                                            ELSE SET @res = -1
                                            
                                            SELECT @res";

        public static string CreateUserForDatabase = @"USE [{0}];
                                                     DECLARE @res INT;
                                                     IF NOT EXISTS (SELECT name FROM sys.database_principals WHERE name = '{1}')
                                                     BEGIN
                                                     CREATE USER [{1}]
                                                     WITH PASSWORD='{2}'
                                                     , DEFAULT_SCHEMA=[dbo];
                                                     SET @res = 0
                                                     END
                                                     ELSE SET @res = -1

                                                     SELECT @res";

        public static string DropUserFromDatabase = @"USE [{0}];
                                                     DECLARE @res INT;
                                                     IF EXISTS (SELECT name FROM sys.database_principals WHERE name = '{1}')
                                                     BEGIN
                                                     DROP USER [{1}]
                                                     SET @res = 0
                                                     END
                                                     ELSE SET @res = -1

                                                     SELECT @res";
    }
}