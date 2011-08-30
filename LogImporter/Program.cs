﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using LogImporter.Properties;

namespace LogImporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = DiscoverFiles();
            ImportFiles(files);
        }

        /// <summary>
        /// Enumerates CSV files in the current directory.
        /// </summary>
        private static IEnumerable<string> DiscoverFiles()
        {
            return Directory.EnumerateFiles("*.csv").Select(f => Path.GetFileName(f));
        }

        /// <summary>
        /// Imports an enumerable list of files into staging, and merges them with the current data.
        /// </summary>
        private static void ImportFiles(IEnumerable<string> csvFiles)
        {
            using (var connection = new SqlConnection(Settings.Default.WebLogs))
            {
                using (var transaction = connection.BeginTransaction())
                {
                    ImportFiles(csvFiles, transaction);
                    transaction.Commit();
                }
            }
        }

        /// <summary>
        /// Imports an enumerable list of files into staging, and merges them with the current data.
        /// </summary>
        /// <remarks>
        /// All operations against the database are performed in the given transaction.
        /// </remarks>
        private static void ImportFiles(IEnumerable<string> csvFiles, SqlTransaction transaction)
        {
            InitializeStagingTable(transaction);

            foreach (var csvFile in csvFiles)
            {
                ImportFile(csvFile, transaction);
            }

            // TODO: Merge the staging table into the main table.
        }

        /// <summary>
        /// Creates the staging table.
        /// </summary>
        private static void InitializeStagingTable(SqlTransaction transaction)
        {
            ExecuteStatement(transaction, Resources.CreateStagingTable);
        }

        /// <summary>
        /// Executes a SQL statement against a transaction.
        /// </summary>
        private static void ExecuteStatement(SqlTransaction transaction, string sql)
        {
            using (var command = transaction.Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Imports a single file into staging.
        /// </summary>
        private static void ImportFile(string csvFile, SqlTransaction transaction)
        {
            // TODO: Import the file into staging.
        }
    }
}
