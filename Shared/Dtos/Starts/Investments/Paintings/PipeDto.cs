using Shared.Dtos.General;
using Shared.Dtos.Starts.Investments;
using Shared.Enums.CostCenter;

namespace Shared.Dtos.Starts.Paintings
{
    public class PaintingDto : GeneralDto  
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid ProjectId { get; set; }

    

        public string Nomenclatore => $"I{Order}";
        public double BudgetUSD { get; set; }

    }
    public class CreatePainting : PaintingDto
    {

    }
    public class EditPainting : PaintingDto
    {

    }
    public class GetAllPaintings
    {
        public Guid ProjectId { get; set; }
    }
    public class GetPaintingById
    {
        public Guid Id { set; get; }
    }
    public class ValidatePaintingName
    {
        public Guid Id { set; get; }
        public string Name { set; get; } = string.Empty;
        public Guid ProjectId { set; get; }
    }

    public class DeletePainting
    {
        public Guid Id { set; get; }
        public Guid ProjectId { set; get; }
    }
    public class DeleteGroupPainting
    {
        public List<Guid> GroupIds { get; set; } = new();
        public Guid ProjectId { set; get; }
    }
    public class ChangeOrderPainting
    {
        public Guid Id { set; get; }
        public int NewOrder { get; set; }
        public Guid ProjectId { set; get; }
    }
}
