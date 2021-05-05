using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Azure.Cosmos;

namespace simpleCrud
{
    class Program
    {
        private const string EndpointUrl = "https://group10cosmosdemo.documents.azure.com:443/";

        private const string AuthorizationKey =
            "0FbU7V5UZZzgbVAG6QGGD5u6oxcLYUhN3MgR1yAiRfCvQ536AQIUoDgig1eQFoQbxPNFLOVBRGqcUExDnCIE2A==";

        private const string DatabaseId = "StudyProgramDatabase";
        private const string ContainerId = "ProgramClassContainer";

        static async Task Main(string[] args)
        {
            CosmosClient cosmosClient = new CosmosClient(EndpointUrl, AuthorizationKey);
            await Program.CreateDatabaseAsync(cosmosClient);
            await Program.CreateContainerAsync(cosmosClient);
            await Program.AddItemsToContainerAsync(cosmosClient);
            //await Program.QueryItemsAsync(cosmosClient);
            //await Program.ReplaceFamilyItemAsync(cosmosClient);
            //await Program.DeleteFamilyItemAsync(cosmosClient);
            //await Program.DeleteDatabaseAndCleanupAsync(cosmosClient);
        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private static async Task CreateDatabaseAsync(CosmosClient cosmosClient)
        {
            // Create a new database
            CosmosDatabase database = await cosmosClient.CreateDatabaseIfNotExistsAsync(Program.DatabaseId);
            Console.WriteLine("Created Database: {0}\n", database.Id);
        }

        /// <summary>
        /// Create the container if it does not exist. 
        /// Specify "/LastName" as the partition key since we're storing family information, to ensure good distribution of requests and storage.
        /// </summary>
        /// <returns></returns>
        private static async Task CreateContainerAsync(CosmosClient cosmosClient)
        {
            // Create a new container
            CosmosContainer container = await cosmosClient.GetDatabase(Program.DatabaseId)
                .CreateContainerIfNotExistsAsync(Program.ContainerId, "/ProgramName");
            Console.WriteLine("Created Container: {0}\n", container.Id);
        }

        /// <summary>
        /// Add Family items to the container
        /// </summary>
        private static async Task AddItemsToContainerAsync(CosmosClient cosmosClient)
        {
            // Create a programClass object for the informatics year one program
            ProgramClass informaticsOne = new ProgramClass
            {
                Id = "Informatics.1",
                ProgramName = "Informatics Year One",
                Lecturers = new Lecturer[]
                {
                    new Lecturer {LecturersName = "Holz"},
                    new Lecturer {LecturersName = "Monsieur"}
                },
                Students = new Student[]
                {
                    new Student
                    {
                        StudentsName = "Peter Bodelschwingh",
                        Gender = "male",
                        CreditsReceived = 90,
                        OverallGrade = 8
                    },
                    new Student
                    {
                        StudentsName = "Patrick Mihaila",
                        Gender = "male",
                        CreditsReceived = 90
                        // Overall Grade = 9
                    }
                },
                Organisation = new Organisation
                {
                    University = "Fontys University of Applied Sciences",
                    BelongingTo = "FHTenL",
                    City = "Venlo"
                },
                IsRegistered = false
            };

            CosmosContainer container = cosmosClient.GetContainer(Program.DatabaseId, Program.ContainerId);
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<ProgramClass> informaticsClassResponse =
                    await container.ReadItemAsync<ProgramClass>(informaticsOne.Id,
                        new PartitionKey(informaticsOne.ProgramName));
                Console.WriteLine("Item in database with id: {0} already exists\n", informaticsClassResponse.Value.Id);
            }
            catch (CosmosException ex) when (ex.Status == (int) HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Informatics ProgramClass. Important: Provide Partition Key (as above)
                ItemResponse<ProgramClass> informaticsClassResponse =
                    await container.CreateItemAsync<ProgramClass>(informaticsOne,
                        new PartitionKey(informaticsOne.ProgramName));
                
                // print response values
                Console.WriteLine("Created item in database with id: {0}\n", informaticsClassResponse.Value.Id);
                Console.WriteLine("Name: {0}\n", informaticsClassResponse.Value.ProgramName);
                // other statements here, such as lectures, students etc 
            }
            
            // Create a informatics object for the informatics year 2 
            ProgramClass informaticsTwo = new ProgramClass
            {
                Id = "Informatics.2",
                ProgramName = "Informatics Year Two",
                Lecturers = new Lecturer[]
                {
                    new Lecturer {LecturersName = "Schwake"},
                    new Lecturer {LecturersName = "van den Ham"}
                },
                Students = new Student[]
                {
                    new Student
                    {
                        StudentsName = "sample Student",
                        Gender = "female",
                        CreditsReceived = 90,
                        OverallGrade = 9
                    },
                    new Student
                    {
                        StudentsName = "another Student",
                        Gender = "male",
                        CreditsReceived = 60,
                        OverallGrade = 10
                    }
                },
                Organisation = new Organisation
                {
                    University = "Fontys University of Applied Sciences",
                    BelongingTo = "FHTenL",
                    City = "Venlo"
                },
                IsRegistered = false
            };

            ItemResponse<ProgramClass> informaticsTwoResponse = await container.UpsertItemAsync<ProgramClass>(informaticsTwo, new PartitionKey(informaticsTwo.ProgramName));
            Console.WriteLine("Created item in database with id: {0}\n", informaticsTwoResponse.Value.Id);
        }

            
        }
    
}