using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Catalogs.Application.Commands.AddToFavorite;
using Catalogs.Application.Commands.DeleteFromFavorite;
using Catalogs.Application.Queries.GetMyFavorites;
using Catalogs.Application.Queries.GetItemIdByFavoriteId;
using Catalogs.Application.Queries.CheckIsFavorite;
using Catalogs.Application.DTOs;
using Core.Pagination;
using Core.Authentication;
using Core.Result;

namespace Catalogs.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoriteController : ControllerBase
{
    private readonly IMediator _mediator;

    public FavoriteController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddToFavorite([FromBody] AddToFavoriteDTO favoriteDto)
    {
        var userId = User.GetId();
        var command = new AddToFavoriteCommand(favoriteDto, userId);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في إضافة العنصر للمفضلة",
                errorType: "AddToFavoriteFailed",
                resultStatus: ResultStatus.Failed));

        return Ok(result);
    }

    [HttpDelete("{itemId}")]
    public async Task<IActionResult> DeleteFromFavorite(Guid itemId)
    {
        var userId = User.GetId();
        var command = new DeleteFromFavoriteCommand(itemId, userId);
        var result = await _mediator.Send(command);

        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في حذف العنصر من المفضلة",
                errorType: "DeleteFromFavoriteFailed",
                resultStatus: ResultStatus.Failed));

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyFavorites([FromQuery] PaginationParameters parameters)
    {
        var userId = User.GetId();
        var query = new GetMyFavoritesQuery(userId, parameters);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب المفضلة",
                errorType: "GetMyFavoritesFailed",
                resultStatus: ResultStatus.Failed));

        return Ok(result);
    }

    [HttpGet("{favoriteId}/item-id")]
    public async Task<IActionResult> GetItemIdByFavoriteId(Guid favoriteId)
    {
        var query = new GetItemIdByFavoriteIdQuery(favoriteId);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في جلب معرف العنصر",
                errorType: "GetItemIdByFavoriteIdFailed",
                resultStatus: ResultStatus.Failed));

        return Ok(result);
    }

    [HttpGet("check/{itemId}")]
    public async Task<IActionResult> CheckIsFavorite(Guid itemId)
    {
        var userId = User.GetId();
        var query = new CheckIsFavoriteQuery(itemId, userId);
        var result = await _mediator.Send(query);

        if (!result.Success)
            return StatusCode(500, Result.Fail(
                message: "فشل في فحص حالة المفضلة",
                errorType: "CheckIsFavoriteFailed",
                resultStatus: ResultStatus.Failed));

        return Ok(result);
    }
}