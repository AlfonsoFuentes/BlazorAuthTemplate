namespace Server.Extensions
{
    //public static class RiskRelationExtensions
    //{
    //    // Método unificado para manejar ambos tipos de riesgo
    //    public static async Task HandleManyToManyRelations(
    //        this object dto, // Recibimos object para chequear interfaces
    //        IAppDbContext context,
    //        Guid newItemId)
    //    {
    //        //1.Vinculación con KnownRisk(La que ya tenías)
    //        if (dto is ILinkableToKnownRisk knownRiskDto &&
    //            knownRiskDto.LinkToKnownRiskId.HasValue &&
    //            knownRiskDto.LinkToKnownRiskId != Guid.Empty)
    //        {
    //            await context.Set<KnownRiskBudgetItem>().AddAsync(new KnownRiskBudgetItem
    //            {
    //                Id = Guid.NewGuid(),
    //                KnownRiskId = knownRiskDto.LinkToKnownRiskId.Value,
    //                BudgetItemId = newItemId
    //            });
    //        }

    //        // 2. ✅ NUEVA: Vinculación con RiskMatrix
    //        if (dto is ILinkableToRiskMatrix riskMatrixDto &&
    //            riskMatrixDto.LinkToRiskMatrixId.HasValue &&
    //            riskMatrixDto.LinkToRiskMatrixId != Guid.Empty)
    //        {
    //            await context.Set<RiskBudgetItem>().AddAsync(new RiskBudgetItem
    //            {
    //                Id = Guid.NewGuid(),
    //                RiskMatrixId = riskMatrixDto.LinkToRiskMatrixId.Value,
    //                BudgetItemId = newItemId
    //            });
    //        }
    //        if (dto is ILinkableToQuality qualitydto &&
    //            qualitydto.LinkToQualityId.HasValue &&
    //            qualitydto.LinkToQualityId != Guid.Empty)
    //        {
    //            await context.Set<QualityBudgetItem>().AddAsync(new QualityBudgetItem
    //            {
    //                Id = Guid.NewGuid(),
    //                QualityId = qualitydto.LinkToQualityId.Value,
    //                BudgetItemId = newItemId,
    //            });
    //        }
    //    }
    //}
    
}
