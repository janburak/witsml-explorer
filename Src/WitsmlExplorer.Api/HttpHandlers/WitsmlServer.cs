using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WitsmlExplorer.Api.Models;
using WitsmlExplorer.Api.Repositories;

namespace WitsmlExplorer.Api.HttpHandler;

public static class WitsmlServerHandler
{
    [Produces(typeof(IEnumerable<Server>))]
    public static async Task<IResult> GetWitsmlServers(IDocumentRepository<Server, Guid> witsmlServerRepository)
    {
        var servers = await witsmlServerRepository.GetDocumentsAsync();
        return Results.Ok(servers);
    }
    [Produces(typeof(Server))]
    public static async Task<IResult> CreateWitsmlServer(Server witsmlServer, IDocumentRepository<Server, Guid> witsmlServerRepository)
    {
        var inserted = await witsmlServerRepository.CreateDocumentAsync(witsmlServer);
        return Results.Ok(inserted);
    }

    [Produces(typeof(Server))]
    public static async Task<IResult> UpdateWitsmlServer(Guid witsmlServerId, Server witsmlServer, IDocumentRepository<Server, Guid> witsmlServerRepository)
    {
        var updatedServer = await witsmlServerRepository.UpdateDocumentAsync(witsmlServerId, witsmlServer);
        return Results.Ok(updatedServer);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public static async Task<IResult> DeleteWitsmlServer(Guid witsmlServerId, IDocumentRepository<Server, Guid> witsmlServerRepository)
    {
        await witsmlServerRepository.DeleteDocumentAsync(witsmlServerId);
        return Results.NoContent();
    }
}