using System;
using System.IO;
using System.Reflection;
using CloudSeedApp;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Xunit;
using Microsoft.Extensions.Configuration;


namespace AppTests {
    public static class TestConfiguration {

        // public static string AppDirectoryPath => GetFullPathFromRelativePath(@"../App");
        // public static string AppDirectoryPath => $"{TestDirectory}/App";

        // public static string TestDirectory = Directory.GetCurrentDirectory();

        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {            
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.Development.json", optional: false)
                .AddEnvironmentVariables()
                .Build();
        }

        private static string GetFullPathFromRelativePath(string relativePath) {
            return Path.GetFullPath(
                Path.Combine(relativePath
            ));
        }
    }
}
