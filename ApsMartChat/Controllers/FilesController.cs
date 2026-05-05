using ApsMartChat.Exceptions;
using ApsMartChat.Services.File;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApsMartChat.Controllers;

[Authorize]
[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IFileService _files;

    public FilesController(IFileService files)
    {
        _files = files;
    }

    // Upload de arquivo para uma sala. Aceita .pdf, .docx, .xlsx (max 200 MB).
    [HttpPost("upload")]
    [RequestSizeLimit(209_715_200)] // 200 MB
    public async Task<IActionResult> Upload([FromForm] List<IFormFile> files, [FromForm] int roomId)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value ?? throw new UnauthorizedException("Token inválido ou sem username");
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var dto = await _files.UploadDeArquivoAsync(files, username, roomId, baseUrl);

        return Ok(dto);
    }

    // Download de um arquivo pelo ID.
    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var result = await _files.DownloadDeArquivoAsync(id);

        var stream = result.stream;
        var TipoConteudo = result.contentType;
        var fileName = result.fileName;

        return File(stream, TipoConteudo, fileName);
    }

    // Lista arquivos de uma sala.
    [HttpGet("room/{roomId:int}")]
    public async Task<IActionResult> GetFilesByRoomAsync(int roomId)
    {
        var files = await _files.GetFilesByRoomAsync(roomId);
        return Ok(files);
    }
}