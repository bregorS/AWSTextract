using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.Textract;
using Amazon.Textract.Model;

namespace Textract;

internal class AnalyseIdDocument
{
    internal static async Task AnalyseDocument()
    {

        var creds = new CredentialProfileStoreChain();

        creds.TryGetAWSCredentials("405802277193_PowerUserAccess", out var awsCreds);

        var docBytes = File.ReadAllBytes("PassportSample.png");

        using var memoryStream = new MemoryStream(docBytes);

        AnalyzeIDRequest request = new AnalyzeIDRequest
        {
            DocumentPages = new List<Document>() { new Document() { Bytes = memoryStream } }
        };

        using var client = new AmazonTextractClient(awsCreds, RegionEndpoint.USEast1);
        var response = await client.AnalyzeIDAsync(request);

        foreach (var block in response.IdentityDocuments)
        {
            foreach (var idField in block.IdentityDocumentFields)
            {
                Console.WriteLine($"Type {idField.Type.Text}, Text: {idField.ValueDetection.Text}");
            }
        }
    }

/*
Example output  

Type FIRST_NAME, Text: ANGELA
Type LAST_NAME, Text: UK SPECIMEN
Type MIDDLE_NAME, Text: ZOE
Type SUFFIX, Text:
Type CITY_IN_ADDRESS, Text:
Type ZIP_CODE_IN_ADDRESS, Text:
Type STATE_IN_ADDRESS, Text:
Type STATE_NAME, Text:
Type DOCUMENT_NUMBER, Text: 533380006
Type EXPIRATION_DATE, Text: 28 SEP /SEPT 25
Type DATE_OF_BIRTH, Text: 04 DEC /DEC 88
Type DATE_OF_ISSUE, Text: 28 SEP /SEPT 15
Type ID_TYPE, Text: PASSPORT
Type ENDORSEMENTS, Text:
Type VETERAN, Text:
Type RESTRICTIONS, Text:
Type CLASS, Text:
Type ADDRESS, Text:
Type COUNTY, Text:
Type PLACE_OF_BIRTH, Text: CROY DON
Type MRZ_CODE, Text: P<GBRUK<SPECIMEN<<ANGELA<ZOE<<<<<<<<<<<<<<<<
5333800068GBR8812049F2509286<<<<<<<<<<<<<<04

*/
}
