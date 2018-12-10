// Required for the injected HttpClient
using System.Net.Http;
using System.Threading.Tasks;
// Required for Blazor
using Microsoft.AspNetCore.Blazor;
using Microsoft.AspNetCore.Blazor.Components;
// Required for the logger/logging
using Microsoft.Extensions.Logging;
using Blazor.Extensions.Logging;
// Access the DTOs defined in a separate assembly, shared with the Console App
using CommonDTOs;
// Use the Serializers from ServiceStack.text
using ServiceStack.Text;
using ServiceStack;
using System;

namespace GUI.Pages {
    public class BasicRESTServicesCodeBehind : BlazorComponent {

        #region string constants
        // Eventually replace with localization
        public const string labelForDataToPost = "Data To Post";
        public const string labelForDataReceivedFromPost = "Data Received from last Post";
        public const string labelForPostDataDataButton = "Press to Post Data";
        public const string dumpPostDataRspDTOText = "Pretry Print the postDataRspDTO object via the ServiceStack.Text extension method Dump: ";
        public const string labelForComplexDataDemo2StringData = "ComplexData.StringData To Post";
        public const string labelForComplexDataDemo2DateTimeData = "ComplexData.DateTimeData To Post";
        public const string labelForComplexDataDemo2IntData = "ComplexData.IntData To Post";
        public const string labelForComplexDataDemo2DoubleData = "ComplexData.DoubleData To Post";
        public const string labelForComplexDataDemo2DecimalData = "ComplexData.DecimalData To Post";
        public const string dumpPostComplexDataReqDTOText = "Pretty Print the ComplexDataReqDTO object via the ServiceStack.Text extension method Dump:";
        public const string labelForComplexDataReceivedFromPost = "ComplexData received from last Post";
        public const string labelForComplexDataReceivedFromPostStringData = "String Data:";
        public const string labelForComplexDataReceivedFromPostDateTimeData = "DateTime (UTC) Data:";
        public const string rspComplexDataDemo2DumpText = "Pretty Print the ComplexDataDemo02 object via the ServiceStack.Text extension method Dump:";
        public const string rspComplexDataDictionaryDemo2DumpText = "Pretty Print the ComplexDataDictionaryDemo02 object via the ServiceStack.Text extension method Dump:";
        public const string labelForComplexPostDataDataButton = "Press to Post ComplexData As a String";
        public const string labelForComplexPostDataDataDictionaryButton = "Press to Post ComplexDataDictionary As a String";
        #endregion

        #region DI container Auto-wired properties
        // This syntax adds to the class a Property that accesses the DI container, and retrieves the instance having the specified type from the DI container.
        // Access the builtin Blazor service that has registered a pre-configured and extended object as a HTTPClient type registered in the DI container
        [Inject]
        HttpClient HttpClient {
            get;
            set;
        }

        // Access the Logging extensions registered in the DI container
        [Inject]
        public ILogger<BasicRESTServicesCodeBehind> Logger {
            get;
            set;
        }

        #endregion

        #region Page Initialization Handler
        protected override async Task OnInitAsync() {
                Logger.LogDebug($"Starting OnInitAsync");
                //Logger.LogDebug($"Initializing IServiceClient");
                // Someday this will work 
                //IServiceClient client = new JsonHttpClient("http://localhost:21100");
                //Logger.LogDebug($"client is null: {client == null}");
                InitializationReqDTO initializationReqDTO = new InitializationReqDTO();

                Logger.LogDebug($"Calling PostJsonAsync<BaseServicesInitializationRspPayload>");
                InitializationRspDTO = await HttpClient.PostJsonAsync<InitializationRspDTO>("Initialization",
                                                                                                                 initializationReqDTO);
                Logger.LogDebug($"Returned from PostJsonAsync<InitializationRspDTO>, InitializationRspDTO = {InitializationRspDTO}");
                // For Demo2, create the instance of the complexDataToPost
                reqComplexDataDemo2 = new ComplexDataDemo2
                {
                    // initialize the fields
                    StringData = "Set By OnInitAsync",
/*                    DateTimeData = DateTime.UtcNow,
                    TimeSpanData = default,
                    IntData = default,
                    DoubleData = default,
                    DecimalData = default
*/
};
                Logger.LogDebug($"In OnInitAsync, complexDataToPost = {reqComplexDataDemo2.Dump()}");
                Logger.LogDebug($"Leaving OnInitAsync");
            }
            //  Create a Property for the Response DTO
            public InitializationRspDTO InitializationRspDTO { get; set; }
        #endregion

        #region Post Data Button OnClick Handler
        public async Task PostData() {
            Logger.LogDebug($"Starting PostData");
                // Create the payload for the Post
                PostDataReqDTO postDataReqDTO = new PostDataReqDTO() { StringDataObject = dataToPost };
            Logger.LogDebug($"Calling PostJsonAsync<PostDataReqDTO> with PostDataReqDTO.StringDataObject = {dataToPost}");
            PostDataRspDTO postDataRspDTO =
              await HttpClient.PostJsonAsync<PostDataRspDTO>("/PostData?format=json", postDataReqDTO);
            Logger.LogDebug($"Returned from PostJsonAsync<PostDataRspDTO> with PostDataRspDTO.StringDataObject = {postDataRspDTO.StringDataObject}");
            dataReceivedFromPost = postDataRspDTO.StringDataObject;
                dumpDataViaSS = postDataRspDTO.Dump();
            Logger.LogDebug($"Leaving PostData");
        }
        #endregion

        #region PostComplexDataAsString OnClick Handler
        public async Task PostComplexDataAsString()
        {
            Logger.LogDebug($"Starting PostComplexDataAsString");
            // Create the payload for the Post. Validation tests on the data entered by the user are not being done, using default types if an input is null
            // Log what is in the page's demo2ComplexDataToPost object
            Logger.LogDebug($"in PostComplexDataAsString: reqComplexDataDemo2: {reqComplexDataDemo2.Dump()}");
            // The HttpClient instance used below came from the DI  (IoC) container. Serialization is at the mercy of whatever Json-serializer the PostJsonAsync is using.
            // To ensure the ServiceStack JSON serializer and deserializer is used, we use SS serializers to serialize to a string, then pass that string to the HttpClient's PostJsonAsync method 
            // Serialize the ComplexData instance object using ServiceStack 
            string reqComplexDataDemo2AsString = ServiceStack.Text.JsonSerializer.SerializeToString<ComplexDataDemo2>(reqComplexDataDemo2);
            Logger.LogDebug($"in PostComplexDataAsString: reqComplexDataDemo2AsString: {reqComplexDataDemo2AsString}");
            // Create an instance of the request DTO class and populate its data property
            ReqComplexDataAsStringDemo2DTO reqComplexDataAsStringDemo2DTO = new ReqComplexDataAsStringDemo2DTO() { ComplexDataAsStringDemo2 = reqComplexDataDemo2AsString };
            // Log the DTO object
            Logger.LogDebug($"in PostComplexDataAsString: reqComplexDataAsStringDemo2DTO: {reqComplexDataAsStringDemo2DTO.Dump()}");
            // pass that object to the PostJsonAsync<string> and await its return 
            // There are no try/catch blocks for error handling in Demo02
            var rspComplexDataAsStringDemo2DTO = await HttpClient.PostJsonAsync<RspComplexDataAsStringDemo2DTO>("/PostComplexDataAsString?format=json", reqComplexDataAsStringDemo2DTO);
            // Don't make any assumptions about what the response was, start by dumping it out for inspection
            Logger.LogDebug($"in PostComplexDataAsString: rspComplexDataDemo2AsStringDTO: {rspComplexDataAsStringDemo2DTO.Dump()}");
            // Print what we expect to be the payload
            string rspComplexDataDemo2AsString = rspComplexDataAsStringDemo2DTO.ComplexDataAsStringDemo2;
            Logger.LogDebug($"in PostComplexDataAsString: rspComplexDataDemo2AsString: {rspComplexDataDemo2AsString}");
            // Deserialize the response from a string to a PostComplexData using the ServiceStack Json-deserializer
            ComplexDataDemo2 rspComplexDataDemo2 = ServiceStack.Text.JsonSerializer.DeserializeFromString<ComplexDataDemo2>(rspComplexDataDemo2AsString);
            // Dump the complex object to a human-readable string
            Logger.LogDebug($"in PostComplexDataAsString: rspComplexDataDemo2: {rspComplexDataDemo2.Dump()}");
            // Set the page's field to display this information on the page
            rspComplexDataDemo2Dump = rspComplexDataDemo2.Dump();
            Logger.LogDebug($"in PostComplexDataAsString dumpComplexDataViaSS: {rspComplexDataDemo2Dump}");
            Logger.LogDebug($"Leaving PostComplexDataAsString");
        }
        #endregion

        #region PostComplexDataDictionaryAsString OnClick Handler
        public async Task PostComplexDataDictionaryAsString()
        {
            Logger.LogDebug($"Starting PostComplexDataDictionaryAsString");
            // Create the payload for the Post. Validation tests on the data entered by the user are not being done, using default types if an input is null
            // Log what is in the page's demo2ComplexDataToPost object
            Logger.LogDebug($"in PostComplexDataDictionaryAsString: reqComplexDataDemo2: {reqComplexDataDemo2.Dump()}");
            // Create a ComplexDataDictionaryDemo2, initialize it with a new dictionary
            ComplexDataDictionaryDemo2 reqComplexDataDictionaryDemo2 = new ComplexDataDictionaryDemo2() { ComplexDataDictDemo2 = new System.Collections.Generic.Dictionary<string, ComplexDataDemo2>() };
            // Log the instance
            Logger.LogDebug($"in PostComplexDataDictionaryAsString: reqComplexDataDictionaryDemo2: {reqComplexDataDictionaryDemo2.Dump()}");
            // Add a Key:Value pair to the empty dictionary, the value is the page's reqComplexDataDemo2 object
            reqComplexDataDictionaryDemo2.ComplexDataDictDemo2["firstkey"] = reqComplexDataDemo2;
            // Log the instance now that it is fully populated
            Logger.LogDebug($"in PostComplexDPostComplexDataDictionaryAsStringataDictionary: reqComplexDataAsDictionaryDemo2: {reqComplexDataDictionaryDemo2.Dump()}");
            // The HttpClient instance used below came from the DI  (IoC) container. Serialization is at the mercy of whatever Json-serializer the PostJsonAsync is using.
            // To ensure the ServiceStack JSON serializer and deserializer is used, we use SS serializers to serialize to a string, then pass that string to the HttpClient's PostJsonAsync method 
            // Serialize the ComplexData instance object using ServiceStack 
            // Convert the ComplexDataDictionary to a string
            var reqComplexDataDictionaryDemo2AsString = ServiceStack.Text.JsonSerializer.SerializeToString<ComplexDataDictionaryDemo2>(reqComplexDataDictionaryDemo2);
            Logger.LogDebug($"in PostComplexDataDictionaryAsString: reqComplexDataDictionaryDemo2AsString: {reqComplexDataDictionaryDemo2AsString}");
            // Create an an instance of the request DTO class and initialize it with the ComplexDataDictionaryDemo2 object we just created
            ReqComplexDataDictionaryAsStringDemo2DTO reqComplexDataDictionaryAsStringDemo2DTO = new ReqComplexDataDictionaryAsStringDemo2DTO() { ComplexDataDictionaryAsStringDemo2 = reqComplexDataDictionaryDemo2AsString };
            // Log the ReqComplexDataDictionaryAsStringDemo2DTO
            Logger.LogDebug($"in PostComplexDataDictionaryAsString: reqComplexDataDictionaryAsStringDemo2DTO: {reqComplexDataDictionaryAsStringDemo2DTO.Dump()}");
            // pass that string to the PostAsync and await its return 
            // There are no try/catch blocks for error handling in Demo02
            var rspComplexDataDictionaryAsStringDemo2DTO = await HttpClient.PostJsonAsync<RspComplexDataDictionaryAsStringDemo2DTO> ("/PostComplexDataDictionaryAsString?format=json", reqComplexDataDictionaryAsStringDemo2DTO);
            // Don't make any assumptions about what the response was, start by dumping it out for inspection
            Logger.LogDebug($"in PostComplexDataDictionaryAsString: rspComplexDataAsStringDemo2DTO: {rspComplexDataDictionaryAsStringDemo2DTO.Dump()}");
            string rspComplexDataAsDictionaryDemo2AsString = rspComplexDataDictionaryAsStringDemo2DTO.ComplexDataDictionaryAsStringDemo2;
            Logger.LogDebug($"in PostComplexDataDictionaryAsString: rspComplexDataAsStringDemo2DTO.ComplexDataAsStringDemo2: {rspComplexDataAsDictionaryDemo2AsString}");
            // Deserialize the response from a string to a PostComplexDataRspDTO using the ServiceStack Json-deserializer
            ComplexDataDictionaryDemo2 rspComplexDataDictionaryDemo2 = ServiceStack.Text.JsonSerializer.DeserializeFromString<ComplexDataDictionaryDemo2>(rspComplexDataAsDictionaryDemo2AsString);
            // Dump the complex object to a human-readable string
            Logger.LogDebug($"in PostComplexDataDictionaryAsString: RspPostComplexData: {rspComplexDataDictionaryDemo2.Dump()}");
            // pretty print it to a string field, and also to the logger
            rspComplexDataDictionaryDemo2Dump = rspComplexDataDictionaryDemo2.Dump();
            Logger.LogDebug($"in PostComplexDataDictionaryAsString dumpComplexDataViaSS: {rspComplexDataDictionaryDemo2Dump}");
            Logger.LogDebug($"Leaving PostComplexDataDictionaryAsString");
        }
        #endregion

        #region public fields

        #region Demo01 code
        public string dataToPost;
      public string dataReceivedFromPost;
        public string dumpDataViaSS;
        #endregion

        #region Demo02 code
        public ComplexDataDemo2 reqComplexDataDemo2;
        public ComplexDataDemo2 rspComplexDataDemo2;
        public string dumpReqDemo2PostComplexData;
        public string rspComplexDataDemo2Dump;
        public string rspComplexDataDictionaryDemo2Dump;
        #endregion
        #endregion
    }
}