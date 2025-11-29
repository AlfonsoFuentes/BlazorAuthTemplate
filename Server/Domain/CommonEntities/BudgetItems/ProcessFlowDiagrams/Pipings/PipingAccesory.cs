namespace Server.Domain.CommonEntities.BudgetItems.ProcessFlowDiagrams.Pipings
{
  //  public class PipingAccesory : Entity
  //  {


  //      public string Name { get; set; } = string.Empty;

  //      public PipingCategory PipingCategory { get; set; } = null!;
  //      public Guid PipingCategoryId { get; set; }

  //      [ForeignKey("PipingAccesoryId")]
  //      public ICollection<IsometricItem> IsometricItems { get; set; } = new List<IsometricItem>();

  //      public ICollection<PipingConnectionType> PipingConnectionTypes { get; set; } = new List<PipingConnectionType>();

  //      [NotMapped]
  //      public PipingConnectionType? LeftConnection => PipingConnectionTypes.Count == 0 || !PipingConnectionTypes.Any(x => x.AccesoryConnectionSide == AccesoryConnectionSide.Left) ?
  //          null! : PipingConnectionTypes.FirstOrDefault(x => x.AccesoryConnectionSide == AccesoryConnectionSide.Left);
  //      [NotMapped]
  //      public PipingConnectionType? RigthConnection => PipingConnectionTypes.Count == 0 || !PipingConnectionTypes.Any(x => x.AccesoryConnectionSide == AccesoryConnectionSide.Right) ?
  //          null! : PipingConnectionTypes.FirstOrDefault(x => x.AccesoryConnectionSide == AccesoryConnectionSide.Right);
  //      [NotMapped]
  //      public PipingConnectionType? BottomConnection => PipingConnectionTypes.Count == 0 || !PipingConnectionTypes.Any(x => x.AccesoryConnectionSide == AccesoryConnectionSide.Bottom) ?
  //  null! : PipingConnectionTypes.FirstOrDefault(x => x.AccesoryConnectionSide == AccesoryConnectionSide.Bottom);

  //      [NotMapped]
  //      public PipingConnectionType? TopConnection => PipingConnectionTypes.Count == 0 || !PipingConnectionTypes.Any(x => x.AccesoryConnectionSide == AccesoryConnectionSide.Top) ?
  //null! : PipingConnectionTypes.FirstOrDefault(x => x.AccesoryConnectionSide == AccesoryConnectionSide.Top);

  //      public PipingConnectionType AddConnection(AccesoryConnectionSide type)
  //      {
  //          return new PipingConnectionType()
  //          {
  //              Id = Guid.NewGuid(),
  //              PipingAccesoryId = Id,
  //              AccesoryConnectionSide = type,
  //          };
  //      }

  //  }

}
