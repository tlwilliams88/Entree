using FluentAssertions;
using Xunit;

using KeithLink.Svc.Core.Models.Lists.Notes;

namespace KeithLink.Svc.Core.Tests.Unit.Models.Lists.Notes {
    public class NotesDetailListTests {
        public static NotesListDetail MakeDetail() {
            return new NotesListDetail {
                Note = "Scooby Dooby Doo"
            };
        }

        public class Get_Note {
            [Fact]
            public void GoodItem_HasDefaultValue() {
                // arrange
                NotesListDetail detail = new NotesListDetail();
                string expected = null;

                // act

                // assert
                detail.Note
                      .Should()
                      .Be(expected);
            }

            [Fact]
            public void GoodItem_ReturnsValue() {
                // arrange
                NotesListDetail detail = MakeDetail();
                string expected = "Scooby Dooby Doo";

                // act

                // assert
                detail.Note
                      .Should()
                      .Be(expected);
            }
        }
    }
}