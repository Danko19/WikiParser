using BLToolkit.DataAccess;
using BLToolkit.Mapping;

namespace WikiParser.MySql
{
    [TableName("abbreviations")]
    public class AbbreviationEntity
    {
        [PrimaryKey]
        [MapField("id")]
        public int Id { get; set; }
        
        [MapField("title")]
        public string Title { get; set; }
        
        [MapField("decryption")]
        public string Decryption { get; set; }
        
        [MapField("link")]
        public string Link { get; set; }
    }
}