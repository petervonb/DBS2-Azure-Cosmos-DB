using System;
using System.Collections.Generic;
using System.IO;
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
        private const string ProgramContainerId = "ProgramClassContainer";
        private const string LecturerContainerId = "LecturerContainer";


        static async Task Main(string[] args)
        {
            CosmosClient cosmosClient = new CosmosClient(EndpointUrl, AuthorizationKey);
            await Program.CreateDatabaseAsync(cosmosClient);
            await Program.CreateContainerAsync(cosmosClient);
            //C
            await Program.AddItemsToProgramClassContainerAsync(cosmosClient);
            await Program.AddItemsToLecturersContainerAsync(cosmosClient);
            await Program.AddDumbItem(cosmosClient);
            //R
            await Program.QueryItemsAsync(cosmosClient);
            // query on Azure Portal
            //U
            await Program.ReplaceProgramClass(cosmosClient);
            //D
            await Program.DeleteSecondYear(cosmosClient);
            // Just dont 
            //await Program.DeleteDatabase(cosmosClient);
        }

        /// Create the database if it does not exist
        private static async Task CreateDatabaseAsync(CosmosClient cosmosClient)
        {
            // Create a new database
            CosmosDatabase database = await cosmosClient.CreateDatabaseIfNotExistsAsync(Program.DatabaseId);
            Console.WriteLine("Created Database: {0}\n", database.Id);
        }

        /// Create the container if it does not exist. 
        private static async Task CreateContainerAsync(CosmosClient cosmosClient)
        {
            // Create a new container
            CosmosContainer container = await cosmosClient.GetDatabase(Program.DatabaseId)
                .CreateContainerIfNotExistsAsync(Program.ProgramContainerId, "/ProgramName");
            Console.WriteLine("Created Container: {0}\n", container.Id);
        }

        /// Add Program Year items to the container
        private static async Task AddItemsToProgramClassContainerAsync(CosmosClient cosmosClient)
        {
            // Create a programClass object for the informatics year one program
            ProgramClass informaticsOne = new ProgramClass
            {
                Id = "Informatics.1",
                ProgramName = "Informatics",
                Year = 1,
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
            };

            CosmosContainer container = cosmosClient.GetContainer(Program.DatabaseId, Program.ProgramContainerId);
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
                ProgramName = "Informatics",
                Year = 2,
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
            };

            ItemResponse<ProgramClass> informaticsTwoResponse =
                await container.UpsertItemAsync<ProgramClass>(informaticsTwo,
                    new PartitionKey(informaticsTwo.ProgramName));
            Console.WriteLine("Created item in database with id: {0}\n", informaticsTwoResponse.Value.Id);
        }

        private static async Task AddItemsToLecturersContainerAsync(CosmosClient cosmosClient)
        {
            // Create a programClass object for the informatics year one program
            Lecturer sampleLecturer = new Lecturer
            {
                Id = "CH1",
                ProgramName = "Informatics",
                LecturersName = "Holz"
            };

            CosmosContainer container = cosmosClient.GetContainer(Program.DatabaseId, Program.LecturerContainerId);
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<Lecturer> lecturerResponse =
                    await container.ReadItemAsync<Lecturer>(sampleLecturer.Id,
                        new PartitionKey(sampleLecturer.LecturersName));
                Console.WriteLine("Item in database with id: {0} already exists\n",
                    lecturerResponse.Value.LecturersName);
            }
            catch (CosmosException ex) when (ex.Status == (int) HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Informatics ProgramClass. Important: Provide Partition Key (as above)
                ItemResponse<Lecturer> lecturerResponse =
                    await container.CreateItemAsync<Lecturer>(sampleLecturer,
                        new PartitionKey(sampleLecturer.LecturersName));

                // print response values
                Console.WriteLine("Created item in database with id: {0}\n", lecturerResponse.Value.LecturersName);
                Console.WriteLine("Name: {0}\n", lecturerResponse.Value.ProgramName);
                // other statements here, such as lectures, students etc 
            }
        }

        private static async Task AddDumbItem(CosmosClient cosmosClient)
        {
            // Create a programClass object for the informatics year one program
            DumbItem sampleItem = new DumbItem
            {
                Id = "DumbItemOne",
                ProgramName = "Informatics",
                ItemName = "This is a dumb Item"
            };

            CosmosContainer container = cosmosClient.GetContainer(Program.DatabaseId, Program.ProgramContainerId);
            try
            {
                // Read the item to see if it exists.  
                ItemResponse<DumbItem> itemResponse =
                    await container.ReadItemAsync<DumbItem>(sampleItem.Id,
                        new PartitionKey(sampleItem.ProgramName));
                Console.WriteLine("Item in database with id: {0} already exists\n",
                    itemResponse.Value.Id);
            }
            catch (CosmosException ex) when (ex.Status == (int) HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Informatics ProgramClass. Important: Provide Partition Key (as above)
                ItemResponse<DumbItem> itemResponse =
                    await container.CreateItemAsync<DumbItem>(sampleItem,
                        new PartitionKey(sampleItem.ProgramName));

                // print response values
                Console.WriteLine("Created item in database with id: {0}\n", itemResponse.Value.Id);
                Console.WriteLine("Name: {0}\n", itemResponse.Value.ProgramName);
                // other statements here, such as lectures, students etc 
            }
        }

        /// Query program container
        private static async Task QueryItemsAsync(CosmosClient cosmosClient)
        {
            var sqlQueryText = "SELECT * FROM c WHERE c.Year = 1";

            Console.WriteLine("Running query: {0}\n", sqlQueryText);

            CosmosContainer container = cosmosClient.GetContainer(Program.DatabaseId, Program.ProgramContainerId);

            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);

            List<ProgramClass> informaticYears = new List<ProgramClass>();

            await foreach (ProgramClass program in container.GetItemQueryIterator<ProgramClass>(queryDefinition))
            {
                informaticYears.Add(program);
                Console.WriteLine("\tRead {0}\n", program);
            }
        }
       
        private static async Task ReplaceProgramClass(CosmosClient cosmosClient)
        {
            CosmosContainer container = cosmosClient.GetContainer(Program.DatabaseId, Program.ProgramContainerId);

            ItemResponse<ProgramClass> informaticsResponse =
                await container.ReadItemAsync<ProgramClass>("Informatics.1", new PartitionKey("Informatics"));
            ProgramClass itemBody = informaticsResponse;

            // update organisation
            itemBody.Organisation.BelongingTo = "FIBS";
            // update grade of a student
            itemBody.Students[0].OverallGrade = 10;

            // replace the item with the updated content
            informaticsResponse =
                await container.ReplaceItemAsync<ProgramClass>(itemBody, itemBody.Id,
                    new PartitionKey(itemBody.ProgramName));
            Console.WriteLine("Updated Program [{0},{1}].\n \tBelongs to {2} now\n ", itemBody.Organisation.BelongingTo,
                itemBody.Id, informaticsResponse.Value);
        }

        private static async Task DeleteSecondYear(CosmosClient cosmosClient)
        {
            CosmosContainer container = cosmosClient.GetContainer(Program.DatabaseId, Program.ProgramContainerId);

            string partitionKeyValue = "Informatics";
            string programId = "Informatics.2";

            // Delete an item. Note we must provide the partition key value and id of the item to delete
            ItemResponse<ProgramClass> informaticsResponse =
                await container.DeleteItemAsync<ProgramClass>(programId, new PartitionKey(partitionKeyValue));
            Console.WriteLine("Deleted Family [{0},{1}]\n", partitionKeyValue, programId);
        }

        private static async Task DeleteDatabase(CosmosClient cosmosClient)
        {
            CosmosDatabase database = cosmosClient.GetDatabase(Program.DatabaseId);
            DatabaseResponse databaseResourceResponse = await database.DeleteAsync();

            Console.WriteLine("Deleted Database: {0}\n", Program.DatabaseId);
        }
    }
}