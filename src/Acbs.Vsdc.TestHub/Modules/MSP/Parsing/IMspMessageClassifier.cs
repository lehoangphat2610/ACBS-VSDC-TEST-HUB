using Acbs.Vsdc.TestHub.Services.Fin;
namespace Acbs.Vsdc.TestHub.Modules.Msp.Parsing;
public interface IMspMessageClassifier { MspClassification Classify(ParsedFinMessage message); }
