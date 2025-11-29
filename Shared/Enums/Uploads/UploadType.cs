using System.ComponentModel;

namespace Shared.Enums.Uploads
{
    public enum UploadType : byte
    {
        [Description(@"Images\Products")]
        Product,

        [Description(@"Images\ProfilePictures")]
        ProfilePicture,

        [Description(@"Documents")]
        Document,

        [Description(@"SAP_Adjust")]
        SAPAdjust
    }
}