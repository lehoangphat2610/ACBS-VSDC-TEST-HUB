using Acbs.Vsdc.TestHub.Services.Fin;
namespace Acbs.Vsdc.TestHub.Modules.Msp.Validation;
public sealed class MspMessageValidator : IMspMessageValidator
{
    public MspValidationResult Validate(ParsedFinMessage p)
    {
        var errors=new List<MspValidationError>(); if(string.IsNullOrWhiteSpace(p.MessageType)) errors.Add(new("T75","Block2","Không xác định được loại điện MT."));
        var reference=p.GetFirst("20") ?? p.GetFirst("20C"); if(string.IsNullOrWhiteSpace(reference)) errors.Add(new("T31","20/20C","Thiếu số tham chiếu."));
        if(p.MessageType=="524") { Require(p,"97A",errors); Require(p,"35B",errors); Require(p,"36B",errors); if(p.GetAll("93A").Count()<2) errors.Add(new("T31","93A","MT524 phải có FROM và TOBA.")); }
        if(p.MessageType is "541" or "543") { foreach(var tag in new[]{"98A","90B","35B","36B","97A","22F","95P","19A"}) Require(p,tag,errors); }
        if(p.MessageType is "199" or "599" && string.IsNullOrWhiteSpace(p.GetFirst("79"))) errors.Add(new("T31","79","Thiếu Narrative."));
        return new(errors);
    }
    private static void Require(ParsedFinMessage p,string tag,List<MspValidationError> errors){if(!p.GetAll(tag).Any())errors.Add(new("T31",tag,$"Thiếu tag {tag}."));}
}
