// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using NovaLab.Data.Models.Twitch.Streams;

namespace NovaLab.Api.Twitch.Streams.ManagedStreamSubject;

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

public record TwitchManagedStreamSubjectDto(
    Guid SubjectId,
    string SelectionName,
    string ObsSubjectTitle,
    string TwitchSubjectTitle
) {
    public static TwitchManagedStreamSubjectDto FromDbObject(TwitchManagedStreamSubject subject) {
        return new TwitchManagedStreamSubjectDto(
            SubjectId: subject.Id,
            SelectionName : subject.SelectionName,
            ObsSubjectTitle : subject.ObsSubjectTitle,
            TwitchSubjectTitle : subject.TwitchSubjectTitle
        );
    }    
} 
