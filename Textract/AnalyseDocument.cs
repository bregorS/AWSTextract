using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.Textract;
using Amazon.Textract.Model;

namespace Textract;

internal class AnalyseFormDocument
{
    internal static async Task AnalyseDocument()
    {
        var creds = new CredentialProfileStoreChain();
        creds.TryGetAWSCredentials("405802277193_PowerUserAccess", out var awsCreds);

        var docBytes = File.ReadAllBytes("P60.jpg");
        using var memoryStream = new MemoryStream(docBytes);

        AnalyzeDocumentRequest analyzeDocumentRequest = new AnalyzeDocumentRequest()
        {
            Document = new Document() { Bytes = memoryStream },
            FeatureTypes = new List<string>() { FeatureType.FORMS }
        };

        using var client = new AmazonTextractClient(awsCreds, RegionEndpoint.USEast1);
        AnalyzeDocumentResponse response = await client.AnalyzeDocumentAsync(analyzeDocumentRequest);

        // https://docs.aws.amazon.com/textract/latest/dg/how-it-works-kvp.html
        var formLabels = new Dictionary<string, string>();
        var formValues = new Dictionary<string, string>();

        foreach (var block in response.Blocks)
        { 
            if (block.BlockType == BlockType.KEY_VALUE_SET)
            {
                if (block.EntityTypes.Contains(EntityType.KEY.ToString()))
                {
                    formLabels.Add(block.Id, block.Text);
                }

                if (block.EntityTypes.Contains(EntityType.VALUE.ToString()))
                {
                    formValues.Add(block.Id, block.Text);
                }
            }
            Console.WriteLine($"The block type is {block.BlockType} and text is {block.Text}");
        }
    }
}