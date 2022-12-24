using System.Reflection.Metadata;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Ozone.Identity.Core.Extensions;
using Ozone.Identity.Core.Security.Jwt;

namespace Ozone.Identity.Core.Tests.Unit.Security.Jwt;

public sealed class JwtTokenSegmentTests {
  public DateTimeOffset TestTimestamp = DateTimeOffset.Parse("16/10/2000 16:00:12");
  public long TestUnixTimestamp = 971704812;

  [Fact]
  public void GetStringEntry_ReturnsEntryValueAsString_WhenSegmentHasEntry() {
    var sut = new JwtTokenSegmentImpl();
    sut.Add("Entry", "Foo");

    sut.GetOptionalEntry<string>("Entry").Should().Be("Foo");
  }

  [Fact]
  public void GetStringEntry_ReturnsNull_WhenSegmentDoesNotHaveEntry() {
    var sut = new JwtTokenSegmentImpl();

    sut.GetOptionalEntry<string>("Entry").Should().BeNull();
  }

  [Fact]
  public void GetRequiredEntryT_ReturnsEntryValueAsT_WhenSegmentHasEntry() {
    var sut = new JwtTokenSegmentImpl();
    sut.Add("Entry", "Foo");

    sut.GetRequiredEntry<string>("Entry").Should().Be("Foo");
  }

  [Fact]
  public void GetRequiredEntryT_ConvertsUnixTimestampToT_WhenTypeIsDateTimeOffsetAndValueIsLong() {
    var sut = new JwtTokenSegmentImpl();
    sut.Add("Date", TestUnixTimestamp);

    sut.GetRequiredEntry<DateTimeOffset>("Date").Should().Be(TestTimestamp);
  }


  [Fact]
  public void GetRequiredEntryT_Throws_WhenSegmentDoesNotHaveEntry() {
    var sut = new JwtTokenSegmentImpl();

    var action = () => sut.GetRequiredEntry<string>("Entry");

    action.Should().Throw<InvalidOperationException>();
  }

  [Fact]
  public void GetRequiredEntryT_ReturnsNull_WhenSegmentDoesNotHaveAsT() {
    var sut = new JwtTokenSegmentImpl();
    sut.Add("Entry", 5);

    var action = sut.GetRequiredEntry<string>("Entry");

    action.Should().BeNull();
  }

  [Fact]
  public void ToSerializedJson_ReturnsEmptyJsonObject_WhenSegmentHasNoEntries() {
    var sut = new JwtTokenSegmentImpl();

    var result = sut.ToSerializedJson();

    result.Should().Be("{}");
  }

  [Fact]
  public void ToSerializedJson_ConvertsDateTimeToUnixTimestamp() {
    var sut = new JwtTokenSegmentImpl();
    var date = TestTimestamp;
    sut.Add("datetime", date);

    var result = sut.ToSerializedJson(Formatting.Indented);
    var expectedUnixDate = EpochTime.GetIntDate(date.UtcDateTime);

    // The auto formatter currently struggles to
    // format raw string literals with interpolation
    // So we statically add TestUnixTimestamp here
    var expected = """
    {
      "datetime": 971704812
    }
    """;
    result.Should().Be(expected);
  }

  [Fact]
  public void ToSerializedJson_ReturnsSerializedJsonWithAllEntries() {
    var sut = new JwtTokenSegmentImpl();
    sut.Add("arr", new[] { "A", "B" });
    sut.Add("datetime", TestTimestamp);
    sut.Add("object", new NestedObject() { Foo = "foo", Bar = "bar" });

    var result = sut.ToSerializedJson(Formatting.Indented);
    var expected = """"
    {
      "arr": [
        "A",
        "B"
      ],
      "datetime": 971704812,
      "object": {
        "Foo": "foo",
        "Bar": "bar"
      }
    }
    """";
    result.Should().Be(expected);
  }

  [Fact]
  public void ToBase64Encoded_ReturnsToSerializedJsonAsBase64WithoutFormatting() {
    var sut = new JwtTokenSegmentImpl();
    sut.Add("arr", new[] { "A", "B" });
    sut.Add("datetime", TestTimestamp);
    sut.Add("object", new NestedObject() { Foo = "foo", Bar = "bar" });

    var expected = sut.ToSerializedJson(Formatting.None).ToBase64UrlEncoded();

    sut.ToBase64Encoded().Should().Be(expected);
  }
}

internal sealed class NestedObject {
  public string Foo { get; init; }
  public string Bar { get; init; }
}

internal sealed class JwtTokenSegmentImpl : JwtTokenSegment { }