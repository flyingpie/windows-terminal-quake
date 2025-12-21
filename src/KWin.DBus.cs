namespace KWin.DBus
{
    using System;
    using Tmds.DBus.Protocol;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    partial class ColorPicker : KWinObject
    {
        private const string __Interface = "org.kde.kwin.ColorPicker";
        public ColorPicker(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<(uint)> PickAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ruz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "pick");
                return writer.CreateMessage();
            }
        }
    }
    record CompositingProperties
    {
        public bool Active { get; set; } = default!;
        public bool CompositingPossible { get; set; } = default!;
        public string CompositingNotPossibleReason { get; set; } = default!;
        public bool OpenGLIsBroken { get; set; } = default!;
        public string CompositingType { get; set; } = default!;
        public string[] SupportedOpenGLPlatformInterfaces { get; set; } = default!;
        public bool PlatformRequiresCompositing { get; set; } = default!;
    }
    partial class Compositing : KWinObject
    {
        private const string __Interface = "org.kde.kwin.Compositing";
        public Compositing(KWinService service, ObjectPath path) : base(service, path)
        { }
        public ValueTask<IDisposable> WatchCompositingToggledAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "compositingToggled", (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public Task<bool> GetActiveAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "active"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetCompositingPossibleAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "compositingPossible"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<string> GetCompositingNotPossibleReasonAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "compositingNotPossibleReason"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<bool> GetOpenGLIsBrokenAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "openGLIsBroken"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<string> GetCompositingTypeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "compositingType"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<string[]> GetSupportedOpenGLPlatformInterfacesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportedOpenGLPlatformInterfaces"), (Message m, object? s) => ReadMessage_v_as(m, (KWinObject)s!), this);
        public Task<bool> GetPlatformRequiresCompositingAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "platformRequiresCompositing"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<CompositingProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static CompositingProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<CompositingProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<CompositingProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<CompositingProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "active": invalidated.Add("Active"); break;
                        case "compositingPossible": invalidated.Add("CompositingPossible"); break;
                        case "compositingNotPossibleReason": invalidated.Add("CompositingNotPossibleReason"); break;
                        case "openGLIsBroken": invalidated.Add("OpenGLIsBroken"); break;
                        case "compositingType": invalidated.Add("CompositingType"); break;
                        case "supportedOpenGLPlatformInterfaces": invalidated.Add("SupportedOpenGLPlatformInterfaces"); break;
                        case "platformRequiresCompositing": invalidated.Add("PlatformRequiresCompositing"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static CompositingProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new CompositingProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "active":
                        reader.ReadSignature("b"u8);
                        props.Active = reader.ReadBool();
                        changedList?.Add("Active");
                        break;
                    case "compositingPossible":
                        reader.ReadSignature("b"u8);
                        props.CompositingPossible = reader.ReadBool();
                        changedList?.Add("CompositingPossible");
                        break;
                    case "compositingNotPossibleReason":
                        reader.ReadSignature("s"u8);
                        props.CompositingNotPossibleReason = reader.ReadString();
                        changedList?.Add("CompositingNotPossibleReason");
                        break;
                    case "openGLIsBroken":
                        reader.ReadSignature("b"u8);
                        props.OpenGLIsBroken = reader.ReadBool();
                        changedList?.Add("OpenGLIsBroken");
                        break;
                    case "compositingType":
                        reader.ReadSignature("s"u8);
                        props.CompositingType = reader.ReadString();
                        changedList?.Add("CompositingType");
                        break;
                    case "supportedOpenGLPlatformInterfaces":
                        reader.ReadSignature("as"u8);
                        props.SupportedOpenGLPlatformInterfaces = reader.ReadArrayOfString();
                        changedList?.Add("SupportedOpenGLPlatformInterfaces");
                        break;
                    case "platformRequiresCompositing":
                        reader.ReadSignature("b"u8);
                        props.PlatformRequiresCompositing = reader.ReadBool();
                        changedList?.Add("PlatformRequiresCompositing");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record EffectsProperties
    {
        public string[] ActiveEffects { get; set; } = default!;
        public string[] LoadedEffects { get; set; } = default!;
        public string[] ListOfEffects { get; set; } = default!;
    }
    partial class Effects : KWinObject
    {
        private const string __Interface = "org.kde.kwin.Effects";
        public Effects(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task ReconfigureEffectAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "reconfigureEffect");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task<bool> LoadEffectAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "loadEffect");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task ToggleEffectAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "toggleEffect");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task UnloadEffectAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "unloadEffect");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task<bool> IsEffectLoadedAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "isEffectLoaded");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task<bool> IsEffectSupportedAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "isEffectSupported");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task<bool[]> AreEffectsSupportedAsync(string[] names)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ab(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "areEffectsSupported");
                writer.WriteArray(names);
                return writer.CreateMessage();
            }
        }
        public Task<string> SupportInformationAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "supportInformation");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task<string> DebugAsync(string name, string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "debug");
                writer.WriteString(name);
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task<string[]> GetActiveEffectsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "activeEffects"), (Message m, object? s) => ReadMessage_v_as(m, (KWinObject)s!), this);
        public Task<string[]> GetLoadedEffectsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "loadedEffects"), (Message m, object? s) => ReadMessage_v_as(m, (KWinObject)s!), this);
        public Task<string[]> GetListOfEffectsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "listOfEffects"), (Message m, object? s) => ReadMessage_v_as(m, (KWinObject)s!), this);
        public Task<EffectsProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static EffectsProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<EffectsProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<EffectsProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<EffectsProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "activeEffects": invalidated.Add("ActiveEffects"); break;
                        case "loadedEffects": invalidated.Add("LoadedEffects"); break;
                        case "listOfEffects": invalidated.Add("ListOfEffects"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static EffectsProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new EffectsProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "activeEffects":
                        reader.ReadSignature("as"u8);
                        props.ActiveEffects = reader.ReadArrayOfString();
                        changedList?.Add("ActiveEffects");
                        break;
                    case "loadedEffects":
                        reader.ReadSignature("as"u8);
                        props.LoadedEffects = reader.ReadArrayOfString();
                        changedList?.Add("LoadedEffects");
                        break;
                    case "listOfEffects":
                        reader.ReadSignature("as"u8);
                        props.ListOfEffects = reader.ReadArrayOfString();
                        changedList?.Add("ListOfEffects");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record FTraceProperties
    {
        public bool IsEnabled { get; set; } = default!;
    }
    partial class FTrace : KWinObject
    {
        private const string __Interface = "org.kde.kwin.FTrace";
        public FTrace(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task SetEnabledAsync(bool enabled)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "b",
                    member: "setEnabled");
                writer.WriteBool(enabled);
                return writer.CreateMessage();
            }
        }
        public Task<bool> GetIsEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "isEnabled"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<FTraceProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static FTraceProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<FTraceProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<FTraceProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<FTraceProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "isEnabled": invalidated.Add("IsEnabled"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static FTraceProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new FTraceProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "isEnabled":
                        reader.ReadSignature("b"u8);
                        props.IsEnabled = reader.ReadBool();
                        changedList?.Add("IsEnabled");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record KWinProperties
    {
        public bool ShowingDesktop { get; set; } = default!;
    }
    partial class KWin : KWinObject
    {
        private const string __Interface = "org.kde.KWin";
        public KWin(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task ReconfigureAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "reconfigure");
                return writer.CreateMessage();
            }
        }
        public Task KillWindowAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "killWindow");
                return writer.CreateMessage();
            }
        }
        public Task<bool> SetCurrentDesktopAsync(int desktop)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "setCurrentDesktop");
                writer.WriteInt32(desktop);
                return writer.CreateMessage();
            }
        }
        public Task<int> CurrentDesktopAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "currentDesktop");
                return writer.CreateMessage();
            }
        }
        public Task NextDesktopAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "nextDesktop");
                return writer.CreateMessage();
            }
        }
        public Task PreviousDesktopAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "previousDesktop");
                return writer.CreateMessage();
            }
        }
        public Task<string> SupportInformationAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "supportInformation");
                return writer.CreateMessage();
            }
        }
        public Task<string> ActiveOutputNameAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_s(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "activeOutputName");
                return writer.CreateMessage();
            }
        }
        public Task ShowDebugConsoleAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "showDebugConsole");
                return writer.CreateMessage();
            }
        }
        public Task ReplaceAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "replace");
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, VariantValue>> QueryWindowInfoAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "queryWindowInfo");
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, VariantValue>> GetWindowInfoAsync(string a0)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "getWindowInfo");
                writer.WriteString(a0);
                return writer.CreateMessage();
            }
        }
        public Task ShowDesktopAsync(bool showing)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "b",
                    member: "showDesktop");
                writer.WriteBool(showing);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchReloadConfigAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "reloadConfig", handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchShowingDesktopChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "showingDesktopChanged", (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public Task<bool> GetShowingDesktopAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "showingDesktop"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<KWinProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static KWinProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<KWinProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<KWinProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<KWinProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "showingDesktop": invalidated.Add("ShowingDesktop"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static KWinProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new KWinProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "showingDesktop":
                        reader.ReadSignature("b"u8);
                        props.ShowingDesktop = reader.ReadBool();
                        changedList?.Add("ShowingDesktop");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class KeyboardLayouts : KWinObject
    {
        private const string __Interface = "org.kde.KeyboardLayouts";
        public KeyboardLayouts(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task SwitchToNextLayoutAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "switchToNextLayout");
                return writer.CreateMessage();
            }
        }
        public Task SwitchToPreviousLayoutAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "switchToPreviousLayout");
                return writer.CreateMessage();
            }
        }
        public Task<bool> SetLayoutAsync(uint index)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "setLayout");
                writer.WriteUInt32(index);
                return writer.CreateMessage();
            }
        }
        public Task<uint> GetLayoutAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "getLayout");
                return writer.CreateMessage();
            }
        }
        public Task<(string, string, string)[]> GetLayoutsListAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_arsssz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "getLayoutsList");
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchLayoutChangedAsync(Action<Exception?, uint> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "layoutChanged", (Message m, object? s) => ReadMessage_u(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchLayoutListChangedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "layoutListChanged", handler, emitOnCapturedContext, flags);
    }
    record PluginsProperties
    {
        public string[] LoadedPlugins { get; set; } = default!;
        public string[] AvailablePlugins { get; set; } = default!;
    }
    partial class Plugins : KWinObject
    {
        private const string __Interface = "org.kde.KWin.Plugins";
        public Plugins(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<bool> LoadPluginAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "LoadPlugin");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task UnloadPluginAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "UnloadPlugin");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task<string[]> GetLoadedPluginsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "LoadedPlugins"), (Message m, object? s) => ReadMessage_v_as(m, (KWinObject)s!), this);
        public Task<string[]> GetAvailablePluginsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "AvailablePlugins"), (Message m, object? s) => ReadMessage_v_as(m, (KWinObject)s!), this);
        public Task<PluginsProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static PluginsProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<PluginsProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<PluginsProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<PluginsProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "LoadedPlugins": invalidated.Add("LoadedPlugins"); break;
                        case "AvailablePlugins": invalidated.Add("AvailablePlugins"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static PluginsProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new PluginsProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "LoadedPlugins":
                        reader.ReadSignature("as"u8);
                        props.LoadedPlugins = reader.ReadArrayOfString();
                        changedList?.Add("LoadedPlugins");
                        break;
                    case "AvailablePlugins":
                        reader.ReadSignature("as"u8);
                        props.AvailablePlugins = reader.ReadArrayOfString();
                        changedList?.Add("AvailablePlugins");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class ScreenSaver : KWinObject
    {
        private const string __Interface = "org.freedesktop.ScreenSaver";
        public ScreenSaver(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task LockAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "Lock");
                return writer.CreateMessage();
            }
        }
        public Task SimulateUserActivityAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "SimulateUserActivity");
                return writer.CreateMessage();
            }
        }
        public Task<bool> GetActiveAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetActive");
                return writer.CreateMessage();
            }
        }
        public Task<uint> GetActiveTimeAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetActiveTime");
                return writer.CreateMessage();
            }
        }
        public Task<uint> GetSessionIdleTimeAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GetSessionIdleTime");
                return writer.CreateMessage();
            }
        }
        public Task<bool> SetActiveAsync(bool e)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "b",
                    member: "SetActive");
                writer.WriteBool(e);
                return writer.CreateMessage();
            }
        }
        public Task<uint> InhibitAsync(string applicationName, string reasonForInhibit)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "Inhibit");
                writer.WriteString(applicationName);
                writer.WriteString(reasonForInhibit);
                return writer.CreateMessage();
            }
        }
        public Task UnInhibitAsync(uint cookie)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "UnInhibit");
                writer.WriteUInt32(cookie);
                return writer.CreateMessage();
            }
        }
        public Task<uint> ThrottleAsync(string applicationName, string reasonForInhibit)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "Throttle");
                writer.WriteString(applicationName);
                writer.WriteString(reasonForInhibit);
                return writer.CreateMessage();
            }
        }
        public Task UnThrottleAsync(uint cookie)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "UnThrottle");
                writer.WriteUInt32(cookie);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchActiveChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "ActiveChanged", (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
    }
    partial class Screensaver : KWinObject
    {
        private const string __Interface = "org.kde.screensaver";
        public Screensaver(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task SwitchUserAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "SwitchUser");
                return writer.CreateMessage();
            }
        }
        public Task ConfigureAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "configure");
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchAboutToLockAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "AboutToLock", handler, emitOnCapturedContext, flags);
    }
    partial class Scripting : KWinObject
    {
        private const string __Interface = "org.kde.kwin.Scripting";
        public Scripting(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task StartAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "start");
                return writer.CreateMessage();
            }
        }
        public Task<int> LoadScriptAsync(string filePath, string pluginName)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "loadScript");
                writer.WriteString(filePath);
                writer.WriteString(pluginName);
                return writer.CreateMessage();
            }
        }
        public Task<int> LoadScriptAsync(string filePath)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "loadScript");
                writer.WriteString(filePath);
                return writer.CreateMessage();
            }
        }
        public Task<int> LoadDeclarativeScriptAsync(string filePath, string pluginName)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "loadDeclarativeScript");
                writer.WriteString(filePath);
                writer.WriteString(pluginName);
                return writer.CreateMessage();
            }
        }
        public Task<int> LoadDeclarativeScriptAsync(string filePath)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_i(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "loadDeclarativeScript");
                writer.WriteString(filePath);
                return writer.CreateMessage();
            }
        }
        public Task<bool> IsScriptLoadedAsync(string pluginName)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "isScriptLoaded");
                writer.WriteString(pluginName);
                return writer.CreateMessage();
            }
        }
        public Task<bool> UnloadScriptAsync(string pluginName)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "unloadScript");
                writer.WriteString(pluginName);
                return writer.CreateMessage();
            }
        }
    }
    partial class Script : KWinObject
    {
        private const string __Interface = "org.kde.kwin.Script";
        public Script(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task StopAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "stop");
                return writer.CreateMessage();
            }
        }
        public Task RunAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "run");
                return writer.CreateMessage();
            }
        }
    }
    partial class Session : KWinObject
    {
        private const string __Interface = "org.kde.KWin.Session";
        public Session(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task SetStateAsync(uint state)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "setState");
                writer.WriteUInt32(state);
                return writer.CreateMessage();
            }
        }
        public Task LoadSessionAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "loadSession");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task AboutToSaveSessionAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "aboutToSaveSession");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task FinishSaveSessionAsync(string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "finishSaveSession");
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task QuitAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "quit");
                return writer.CreateMessage();
            }
        }
        public Task<bool> CloseWaylandWindowsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "closeWaylandWindows");
                return writer.CreateMessage();
            }
        }
    }
    record VirtualDesktopManagerProperties
    {
        public uint Count { get; set; } = default!;
        public string Current { get; set; } = default!;
        public uint Rows { get; set; } = default!;
        public bool NavigationWrappingAround { get; set; } = default!;
        public (int, string, string)[] Desktops { get; set; } = default!;
    }
    partial class VirtualDesktopManager : KWinObject
    {
        private const string __Interface = "org.kde.KWin.VirtualDesktopManager";
        public VirtualDesktopManager(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task CreateDesktopAsync(uint position, string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "us",
                    member: "createDesktop");
                writer.WriteUInt32(position);
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task SetDesktopNameAsync(string id, string name)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "setDesktopName");
                writer.WriteString(id);
                writer.WriteString(name);
                return writer.CreateMessage();
            }
        }
        public Task RemoveDesktopAsync(string id)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "removeDesktop");
                writer.WriteString(id);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchCountChangedAsync(Action<Exception?, uint> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "countChanged", (Message m, object? s) => ReadMessage_u(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchRowsChangedAsync(Action<Exception?, uint> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "rowsChanged", (Message m, object? s) => ReadMessage_u(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchCurrentChangedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "currentChanged", (Message m, object? s) => ReadMessage_s(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchNavigationWrappingAroundChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "navigationWrappingAroundChanged", (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchDesktopDataChangedAsync(Action<Exception?, (string Id, (int, string, string) DesktopData)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "desktopDataChanged", (Message m, object? s) => ReadMessage_srissz(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchDesktopCreatedAsync(Action<Exception?, (string Id, (int, string, string) DesktopData)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "desktopCreated", (Message m, object? s) => ReadMessage_srissz(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchDesktopRemovedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "desktopRemoved", (Message m, object? s) => ReadMessage_s(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public Task SetCurrentAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("current");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetRowsAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("rows");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        public Task SetNavigationWrappingAroundAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("navigationWrappingAround");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task<uint> GetCountAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "count"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<string> GetCurrentAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "current"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<uint> GetRowsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "rows"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<bool> GetNavigationWrappingAroundAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "navigationWrappingAround"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<(int, string, string)[]> GetDesktopsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "desktops"), (Message m, object? s) => ReadMessage_v_arissz(m, (KWinObject)s!), this);
        public Task<VirtualDesktopManagerProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static VirtualDesktopManagerProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<VirtualDesktopManagerProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<VirtualDesktopManagerProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<VirtualDesktopManagerProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "count": invalidated.Add("Count"); break;
                        case "current": invalidated.Add("Current"); break;
                        case "rows": invalidated.Add("Rows"); break;
                        case "navigationWrappingAround": invalidated.Add("NavigationWrappingAround"); break;
                        case "desktops": invalidated.Add("Desktops"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static VirtualDesktopManagerProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new VirtualDesktopManagerProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "count":
                        reader.ReadSignature("u"u8);
                        props.Count = reader.ReadUInt32();
                        changedList?.Add("Count");
                        break;
                    case "current":
                        reader.ReadSignature("s"u8);
                        props.Current = reader.ReadString();
                        changedList?.Add("Current");
                        break;
                    case "rows":
                        reader.ReadSignature("u"u8);
                        props.Rows = reader.ReadUInt32();
                        changedList?.Add("Rows");
                        break;
                    case "navigationWrappingAround":
                        reader.ReadSignature("b"u8);
                        props.NavigationWrappingAround = reader.ReadBool();
                        changedList?.Add("NavigationWrappingAround");
                        break;
                    case "desktops":
                        reader.ReadSignature("a(iss)"u8);
                        props.Desktops = ReadType_arissz(ref reader);
                        changedList?.Add("Desktops");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record VirtualKeyboardProperties
    {
        public bool Available { get; set; } = default!;
        public bool Enabled { get; set; } = default!;
        public bool Active { get; set; } = default!;
        public bool Visible { get; set; } = default!;
        public bool ActiveClientSupportsTextInput { get; set; } = default!;
    }
    partial class VirtualKeyboard : KWinObject
    {
        private const string __Interface = "org.kde.kwin.VirtualKeyboard";
        public VirtualKeyboard(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<bool> WillShowOnActiveAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "willShowOnActive");
                return writer.CreateMessage();
            }
        }
        public Task ForceActivateAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "forceActivate");
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchEnabledChangedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "enabledChanged", handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchActiveChangedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "activeChanged", handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchVisibleChangedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "visibleChanged", handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchAvailableChangedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "availableChanged", handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchActiveClientSupportsTextInputChangedAsync(Action<Exception?> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "activeClientSupportsTextInputChanged", handler, emitOnCapturedContext, flags);
        public Task SetEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("enabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetActiveAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("active");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task<bool> GetAvailableAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "available"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "enabled"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetActiveAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "active"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetVisibleAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "visible"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetActiveClientSupportsTextInputAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "activeClientSupportsTextInput"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<VirtualKeyboardProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static VirtualKeyboardProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<VirtualKeyboardProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<VirtualKeyboardProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<VirtualKeyboardProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "available": invalidated.Add("Available"); break;
                        case "enabled": invalidated.Add("Enabled"); break;
                        case "active": invalidated.Add("Active"); break;
                        case "visible": invalidated.Add("Visible"); break;
                        case "activeClientSupportsTextInput": invalidated.Add("ActiveClientSupportsTextInput"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static VirtualKeyboardProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new VirtualKeyboardProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "available":
                        reader.ReadSignature("b"u8);
                        props.Available = reader.ReadBool();
                        changedList?.Add("Available");
                        break;
                    case "enabled":
                        reader.ReadSignature("b"u8);
                        props.Enabled = reader.ReadBool();
                        changedList?.Add("Enabled");
                        break;
                    case "active":
                        reader.ReadSignature("b"u8);
                        props.Active = reader.ReadBool();
                        changedList?.Add("Active");
                        break;
                    case "visible":
                        reader.ReadSignature("b"u8);
                        props.Visible = reader.ReadBool();
                        changedList?.Add("Visible");
                        break;
                    case "activeClientSupportsTextInput":
                        reader.ReadSignature("b"u8);
                        props.ActiveClientSupportsTextInput = reader.ReadBool();
                        changedList?.Add("ActiveClientSupportsTextInput");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class Krunner1 : KWinObject
    {
        private const string __Interface = "org.kde.krunner1";
        public Krunner1(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<(string, string, string)[]> ActionsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_arsssz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "Actions");
                return writer.CreateMessage();
            }
        }
        public Task RunAsync(string matchId, string actionId)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "Run");
                writer.WriteString(matchId);
                writer.WriteString(actionId);
                return writer.CreateMessage();
            }
        }
        public Task<(string, string, string, uint, double, Dictionary<string, VariantValue>)[]> MatchAsync(string query)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_arsssudaesvz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "Match");
                writer.WriteString(query);
                return writer.CreateMessage();
            }
        }
    }
    record ComponentProperties
    {
        public string FriendlyName { get; set; } = default!;
        public string UniqueName { get; set; } = default!;
    }
    partial class Component : KWinObject
    {
        private const string __Interface = "org.kde.kglobalaccel.Component";
        public Component(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<bool> CleanUpAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "cleanUp");
                return writer.CreateMessage();
            }
        }
        public Task<bool> IsActiveAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "isActive");
                return writer.CreateMessage();
            }
        }
        public Task<string[]> ShortcutNamesAsync(string context)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_as(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "shortcutNames");
                writer.WriteString(context);
                return writer.CreateMessage();
            }
        }
        public Task<string[]> ShortcutNamesAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_as(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "shortcutNames");
                return writer.CreateMessage();
            }
        }
        public Task<(string, string, string, string, string, string, int[], int[])[]> AllShortcutInfosAsync(string context)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_arssssssaiaiz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "allShortcutInfos");
                writer.WriteString(context);
                return writer.CreateMessage();
            }
        }
        public Task<(string, string, string, string, string, string, int[], int[])[]> AllShortcutInfosAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_arssssssaiaiz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "allShortcutInfos");
                return writer.CreateMessage();
            }
        }
        public Task<string[]> GetShortcutContextsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_as(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "getShortcutContexts");
                return writer.CreateMessage();
            }
        }
        public Task InvokeShortcutAsync(string shortcutName, string context)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "invokeShortcut");
                writer.WriteString(shortcutName);
                writer.WriteString(context);
                return writer.CreateMessage();
            }
        }
        public Task InvokeShortcutAsync(string shortcutName)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "invokeShortcut");
                writer.WriteString(shortcutName);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchGlobalShortcutPressedAsync(Action<Exception?, (string ComponentUnique, string ShortcutUnique, long Timestamp)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "globalShortcutPressed", (Message m, object? s) => ReadMessage_ssx(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchGlobalShortcutRepeatedAsync(Action<Exception?, (string ComponentUnique, string ShortcutUnique, long Timestamp)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "globalShortcutRepeated", (Message m, object? s) => ReadMessage_ssx(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchGlobalShortcutReleasedAsync(Action<Exception?, (string ComponentUnique, string ShortcutUnique, long Timestamp)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "globalShortcutReleased", (Message m, object? s) => ReadMessage_ssx(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public Task<string> GetFriendlyNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "friendlyName"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<string> GetUniqueNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "uniqueName"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<ComponentProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static ComponentProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<ComponentProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<ComponentProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<ComponentProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "friendlyName": invalidated.Add("FriendlyName"); break;
                        case "uniqueName": invalidated.Add("UniqueName"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static ComponentProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new ComponentProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "friendlyName":
                        reader.ReadSignature("s"u8);
                        props.FriendlyName = reader.ReadString();
                        changedList?.Add("FriendlyName");
                        break;
                    case "uniqueName":
                        reader.ReadSignature("s"u8);
                        props.UniqueName = reader.ReadString();
                        changedList?.Add("UniqueName");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class KGlobalAccel : KWinObject
    {
        private const string __Interface = "org.kde.KGlobalAccel";
        public KGlobalAccel(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<ObjectPath[]> AllComponentsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ao(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "allComponents");
                return writer.CreateMessage();
            }
        }
        public Task<string[][]> AllMainComponentsAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aas(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "allMainComponents");
                return writer.CreateMessage();
            }
        }
        public Task<string[][]> AllActionsForComponentAsync(string[] actionId)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aas(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "allActionsForComponent");
                writer.WriteArray(actionId);
                return writer.CreateMessage();
            }
        }
        public Task<string[]> ActionAsync(int key)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_as(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "action");
                writer.WriteInt32(key);
                return writer.CreateMessage();
            }
        }
        public Task<string[]> ActionListAsync((int[]) key)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_as(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "(ai)",
                    member: "actionList");
                WriteType_raiz(ref writer, key);
                return writer.CreateMessage();
            }
        }
        public Task<int[]> ShortcutAsync(string[] actionId)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ai(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "shortcut");
                writer.WriteArray(actionId);
                return writer.CreateMessage();
            }
        }
        public Task<(int[])[]> ShortcutKeysAsync(string[] actionId)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_araiz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "shortcutKeys");
                writer.WriteArray(actionId);
                return writer.CreateMessage();
            }
        }
        public Task<int[]> DefaultShortcutAsync(string[] actionId)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ai(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "defaultShortcut");
                writer.WriteArray(actionId);
                return writer.CreateMessage();
            }
        }
        public Task<(int[])[]> DefaultShortcutKeysAsync(string[] actionId)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_araiz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "defaultShortcutKeys");
                writer.WriteArray(actionId);
                return writer.CreateMessage();
            }
        }
        public Task<ObjectPath> GetComponentAsync(string componentUnique)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_o(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "s",
                    member: "getComponent");
                writer.WriteString(componentUnique);
                return writer.CreateMessage();
            }
        }
        public Task<int[]> SetShortcutAsync(string[] actionId, int[] keys, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_ai(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "asaiu",
                    member: "setShortcut");
                writer.WriteArray(actionId);
                writer.WriteArray(keys);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task<(int[])[]> SetShortcutKeysAsync(string[] actionId, (int[])[] keys, uint flags)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_araiz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "asa(ai)u",
                    member: "setShortcutKeys");
                writer.WriteArray(actionId);
                WriteType_araiz(ref writer, keys);
                writer.WriteUInt32(flags);
                return writer.CreateMessage();
            }
        }
        public Task SetForeignShortcutAsync(string[] actionId, int[] keys)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "asai",
                    member: "setForeignShortcut");
                writer.WriteArray(actionId);
                writer.WriteArray(keys);
                return writer.CreateMessage();
            }
        }
        public Task SetForeignShortcutKeysAsync(string[] actionId, (int[])[] keys)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "asa(ai)",
                    member: "setForeignShortcutKeys");
                writer.WriteArray(actionId);
                WriteType_araiz(ref writer, keys);
                return writer.CreateMessage();
            }
        }
        public Task SetInactiveAsync(string[] actionId)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "setInactive");
                writer.WriteArray(actionId);
                return writer.CreateMessage();
            }
        }
        public Task DoRegisterAsync(string[] actionId)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "doRegister");
                writer.WriteArray(actionId);
                return writer.CreateMessage();
            }
        }
        public Task UnRegisterAsync(string[] actionId)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "unRegister");
                writer.WriteArray(actionId);
                return writer.CreateMessage();
            }
        }
        public Task ActivateGlobalShortcutContextAsync(string component, string context)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "activateGlobalShortcutContext");
                writer.WriteString(component);
                writer.WriteString(context);
                return writer.CreateMessage();
            }
        }
        public Task<(string, string, string, string, string, string, int[], int[])[]> GetGlobalShortcutsByKeyAsync(int key)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_arssssssaiaiz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "getGlobalShortcutsByKey");
                writer.WriteInt32(key);
                return writer.CreateMessage();
            }
        }
        public Task<(string, string, string, string, string, string, int[], int[])[]> GlobalShortcutsByKeyAsync((int[]) key, (int) @type)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_arssssssaiaiz(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "(ai)(i)",
                    member: "globalShortcutsByKey");
                WriteType_raiz(ref writer, key);
                WriteType_riz(ref writer, @type);
                return writer.CreateMessage();
            }
        }
        public Task<bool> IsGlobalShortcutAvailableAsync(int key, string component)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "is",
                    member: "isGlobalShortcutAvailable");
                writer.WriteInt32(key);
                writer.WriteString(component);
                return writer.CreateMessage();
            }
        }
        public Task<bool> GlobalShortcutAvailableAsync((int[]) key, string component)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "(ai)s",
                    member: "globalShortcutAvailable");
                WriteType_raiz(ref writer, key);
                writer.WriteString(component);
                return writer.CreateMessage();
            }
        }
        public Task<bool> UnregisterAsync(string componentUnique, string shortcutUnique)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ss",
                    member: "unregister");
                writer.WriteString(componentUnique);
                writer.WriteString(shortcutUnique);
                return writer.CreateMessage();
            }
        }
        public Task BlockGlobalShortcutsAsync(bool a0)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "b",
                    member: "blockGlobalShortcuts");
                writer.WriteBool(a0);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchYourShortcutGotChangedAsync(Action<Exception?, (string[] ActionId, int[] NewKeys)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "yourShortcutGotChanged", (Message m, object? s) => ReadMessage_asai(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchYourShortcutsChangedAsync(Action<Exception?, (string[] ActionId, (int[])[] NewKeys)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "yourShortcutsChanged", (Message m, object? s) => ReadMessage_asaraiz(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
    }
    partial class Touchpad : KWinObject
    {
        private const string __Interface = "org.kde.touchpad";
        public Touchpad(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task ToggleAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "toggle");
                return writer.CreateMessage();
            }
        }
        public Task DisableAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "disable");
                return writer.CreateMessage();
            }
        }
        public Task EnableAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "enable");
                return writer.CreateMessage();
            }
        }
    }
    partial class KeyboardMonitor : KWinObject
    {
        private const string __Interface = "org.freedesktop.a11y.KeyboardMonitor";
        public KeyboardMonitor(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task GrabKeyboardAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "GrabKeyboard");
                return writer.CreateMessage();
            }
        }
        public Task UngrabKeyboardAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "UngrabKeyboard");
                return writer.CreateMessage();
            }
        }
        public Task WatchKeyboardAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "WatchKeyboard");
                return writer.CreateMessage();
            }
        }
        public Task UnwatchKeyboardAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "UnwatchKeyboard");
                return writer.CreateMessage();
            }
        }
        public Task SetKeyGrabsAsync(uint[] modifiers, (uint, uint)[] keystrokes)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "aua(uu)",
                    member: "SetKeyGrabs");
                writer.WriteArray(modifiers);
                WriteType_aruuz(ref writer, keystrokes);
                return writer.CreateMessage();
            }
        }
        public ValueTask<IDisposable> WatchKeyEventAsync(Action<Exception?, (bool Released, uint State, uint Keysym, uint Unichar, ushort Keycode)> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "KeyEvent", (Message m, object? s) => ReadMessage_buuuq(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
    }
    record TabletModeManagerProperties
    {
        public bool TabletModeAvailable { get; set; } = default!;
        public bool TabletMode { get; set; } = default!;
    }
    partial class TabletModeManager : KWinObject
    {
        private const string __Interface = "org.kde.KWin.TabletModeManager";
        public TabletModeManager(KWinService service, ObjectPath path) : base(service, path)
        { }
        public ValueTask<IDisposable> WatchTabletModeAvailableChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "tabletModeAvailableChanged", (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchTabletModeChangedAsync(Action<Exception?, bool> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "tabletModeChanged", (Message m, object? s) => ReadMessage_b(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public Task<bool> GetTabletModeAvailableAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletModeAvailable"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTabletModeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletMode"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<TabletModeManagerProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static TabletModeManagerProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<TabletModeManagerProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<TabletModeManagerProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<TabletModeManagerProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "tabletModeAvailable": invalidated.Add("TabletModeAvailable"); break;
                        case "tabletMode": invalidated.Add("TabletMode"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static TabletModeManagerProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new TabletModeManagerProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "tabletModeAvailable":
                        reader.ReadSignature("b"u8);
                        props.TabletModeAvailable = reader.ReadBool();
                        changedList?.Add("TabletModeAvailable");
                        break;
                    case "tabletMode":
                        reader.ReadSignature("b"u8);
                        props.TabletMode = reader.ReadBool();
                        changedList?.Add("TabletMode");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class BlendChanges : KWinObject
    {
        private const string __Interface = "org.kde.KWin.BlendChanges";
        public BlendChanges(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task StartAsync(int delay)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "start");
                writer.WriteInt32(delay);
                return writer.CreateMessage();
            }
        }
        public Task StartAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "start");
                return writer.CreateMessage();
            }
        }
    }
    partial class InputCaptureManager : KWinObject
    {
        private const string __Interface = "org.kde.KWin.EIS.InputCaptureManager";
        public InputCaptureManager(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<ObjectPath> AddInputCaptureAsync(uint capabilities)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_o(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "addInputCapture");
                writer.WriteUInt32(capabilities);
                return writer.CreateMessage();
            }
        }
        public Task RemoveInputCaptureAsync(ObjectPath capture)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "o",
                    member: "removeInputCapture");
                writer.WriteObjectPath(capture);
                return writer.CreateMessage();
            }
        }
    }
    partial class RemoteDesktop : KWinObject
    {
        private const string __Interface = "org.kde.KWin.EIS.RemoteDesktop";
        public RemoteDesktop(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<(System.Runtime.InteropServices.SafeHandle A0, int Cookie)> ConnectToEISAsync(int capabilities)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_hi(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "connectToEIS");
                writer.WriteInt32(capabilities);
                return writer.CreateMessage();
            }
        }
        public Task DisconnectAsync(int cookie)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "i",
                    member: "disconnect");
                writer.WriteInt32(cookie);
                return writer.CreateMessage();
            }
        }
    }
    partial class OutputLocator1 : KWinObject
    {
        private const string __Interface = "org.kde.KWin.Effect.OutputLocator1";
        public OutputLocator1(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task ShowAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "show");
                return writer.CreateMessage();
            }
        }
        public Task HideAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "hide");
                return writer.CreateMessage();
            }
        }
    }
    partial class SystemBell1 : KWinObject
    {
        private const string __Interface = "org.kde.KWin.Effect.SystemBell1";
        public SystemBell1(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task TriggerScreenAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "triggerScreen");
                return writer.CreateMessage();
            }
        }
        public Task TriggerWindowAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "triggerWindow");
                return writer.CreateMessage();
            }
        }
    }
    partial class WindowView1 : KWinObject
    {
        private const string __Interface = "org.kde.KWin.Effect.WindowView1";
        public WindowView1(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task ActivateAsync(string[] handles)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "activate");
                writer.WriteArray(handles);
                return writer.CreateMessage();
            }
        }
    }
    partial class HighlightWindow : KWinObject
    {
        private const string __Interface = "org.kde.KWin.HighlightWindow";
        public HighlightWindow(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task HighlightWindowsAsync(string[] windows)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "as",
                    member: "highlightWindows");
                writer.WriteArray(windows);
                return writer.CreateMessage();
            }
        }
    }
    record InputDeviceManagerProperties
    {
        public string[] DevicesSysNames { get; set; } = default!;
    }
    partial class InputDeviceManager : KWinObject
    {
        private const string __Interface = "org.kde.KWin.InputDeviceManager";
        public InputDeviceManager(KWinService service, ObjectPath path) : base(service, path)
        { }
        public ValueTask<IDisposable> WatchDeviceAddedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "deviceAdded", (Message m, object? s) => ReadMessage_s(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public ValueTask<IDisposable> WatchDeviceRemovedAsync(Action<Exception?, string> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
            => base.WatchSignalAsync(Service.Destination, __Interface, Path, "deviceRemoved", (Message m, object? s) => ReadMessage_s(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
        public Task<string[]> GetDevicesSysNamesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "devicesSysNames"), (Message m, object? s) => ReadMessage_v_as(m, (KWinObject)s!), this);
        public Task<InputDeviceManagerProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static InputDeviceManagerProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<InputDeviceManagerProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<InputDeviceManagerProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<InputDeviceManagerProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "devicesSysNames": invalidated.Add("DevicesSysNames"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static InputDeviceManagerProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new InputDeviceManagerProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "devicesSysNames":
                        reader.ReadSignature("as"u8);
                        props.DevicesSysNames = reader.ReadArrayOfString();
                        changedList?.Add("DevicesSysNames");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record InputDeviceProperties
    {
        public bool Keyboard { get; set; } = default!;
        public bool AlphaNumericKeyboard { get; set; } = default!;
        public bool Pointer { get; set; } = default!;
        public bool Touchpad { get; set; } = default!;
        public bool Touch { get; set; } = default!;
        public bool TabletTool { get; set; } = default!;
        public bool TabletPad { get; set; } = default!;
        public bool GestureSupport { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string SysName { get; set; } = default!;
        public string OutputName { get; set; } = default!;
        public (double, double) Size { get; set; } = default!;
        public uint Product { get; set; } = default!;
        public uint Vendor { get; set; } = default!;
        public bool SupportsDisableEvents { get; set; } = default!;
        public bool Enabled { get; set; } = default!;
        public bool EnabledByDefault { get; set; } = default!;
        public int SupportedButtons { get; set; } = default!;
        public bool SupportsCalibrationMatrix { get; set; } = default!;
        public string DefaultCalibrationMatrix { get; set; } = default!;
        public string CalibrationMatrix { get; set; } = default!;
        public int OrientationDBus { get; set; } = default!;
        public bool SupportsLeftHanded { get; set; } = default!;
        public bool LeftHandedEnabledByDefault { get; set; } = default!;
        public bool LeftHanded { get; set; } = default!;
        public bool SupportsDisableEventsOnExternalMouse { get; set; } = default!;
        public bool DisableEventsOnExternalMouseEnabledByDefault { get; set; } = default!;
        public bool DisableEventsOnExternalMouse { get; set; } = default!;
        public bool SupportsDisableWhileTyping { get; set; } = default!;
        public bool DisableWhileTypingEnabledByDefault { get; set; } = default!;
        public bool DisableWhileTyping { get; set; } = default!;
        public bool SupportsPointerAcceleration { get; set; } = default!;
        public double DefaultPointerAcceleration { get; set; } = default!;
        public double PointerAcceleration { get; set; } = default!;
        public bool SupportsPointerAccelerationProfileFlat { get; set; } = default!;
        public bool DefaultPointerAccelerationProfileFlat { get; set; } = default!;
        public bool PointerAccelerationProfileFlat { get; set; } = default!;
        public bool SupportsPointerAccelerationProfileAdaptive { get; set; } = default!;
        public bool DefaultPointerAccelerationProfileAdaptive { get; set; } = default!;
        public bool PointerAccelerationProfileAdaptive { get; set; } = default!;
        public int TapFingerCount { get; set; } = default!;
        public bool TapToClickEnabledByDefault { get; set; } = default!;
        public bool TapToClick { get; set; } = default!;
        public bool SupportsLmrTapButtonMap { get; set; } = default!;
        public bool LmrTapButtonMapEnabledByDefault { get; set; } = default!;
        public bool LmrTapButtonMap { get; set; } = default!;
        public bool TapAndDragEnabledByDefault { get; set; } = default!;
        public bool TapAndDrag { get; set; } = default!;
        public bool TapDragLockEnabledByDefault { get; set; } = default!;
        public bool TapDragLock { get; set; } = default!;
        public bool SupportsMiddleEmulation { get; set; } = default!;
        public bool MiddleEmulationEnabledByDefault { get; set; } = default!;
        public bool MiddleEmulation { get; set; } = default!;
        public bool SupportsNaturalScroll { get; set; } = default!;
        public bool NaturalScrollEnabledByDefault { get; set; } = default!;
        public bool NaturalScroll { get; set; } = default!;
        public bool SupportsScrollTwoFinger { get; set; } = default!;
        public bool ScrollTwoFingerEnabledByDefault { get; set; } = default!;
        public bool ScrollTwoFinger { get; set; } = default!;
        public bool SupportsScrollEdge { get; set; } = default!;
        public bool ScrollEdgeEnabledByDefault { get; set; } = default!;
        public bool ScrollEdge { get; set; } = default!;
        public bool SupportsScrollOnButtonDown { get; set; } = default!;
        public bool ScrollOnButtonDownEnabledByDefault { get; set; } = default!;
        public uint DefaultScrollButton { get; set; } = default!;
        public bool ScrollOnButtonDown { get; set; } = default!;
        public uint ScrollButton { get; set; } = default!;
        public double ScrollFactor { get; set; } = default!;
        public bool SwitchDevice { get; set; } = default!;
        public bool LidSwitch { get; set; } = default!;
        public bool TabletModeSwitch { get; set; } = default!;
        public bool SupportsClickMethodAreas { get; set; } = default!;
        public bool DefaultClickMethodAreas { get; set; } = default!;
        public bool ClickMethodAreas { get; set; } = default!;
        public bool SupportsClickMethodClickfinger { get; set; } = default!;
        public bool DefaultClickMethodClickfinger { get; set; } = default!;
        public bool ClickMethodClickfinger { get; set; } = default!;
        public bool SupportsOutputArea { get; set; } = default!;
        public (double, double, double, double) DefaultOutputArea { get; set; } = default!;
        public (double, double, double, double) OutputArea { get; set; } = default!;
        public bool DefaultMapToWorkspace { get; set; } = default!;
        public bool MapToWorkspace { get; set; } = default!;
        public string DeviceGroupId { get; set; } = default!;
        public string DefaultPressureCurve { get; set; } = default!;
        public string PressureCurve { get; set; } = default!;
        public uint TabletPadButtonCount { get; set; } = default!;
        public uint TabletPadDialCount { get; set; } = default!;
        public uint TabletPadRingCount { get; set; } = default!;
        public uint TabletPadStripCount { get; set; } = default!;
        public bool SupportsInputArea { get; set; } = default!;
        public (double, double, double, double) DefaultInputArea { get; set; } = default!;
        public (double, double, double, double) InputArea { get; set; } = default!;
        public uint[] NumModes { get; set; } = default!;
        public uint[] CurrentModes { get; set; } = default!;
        public bool SupportsPressureRange { get; set; } = default!;
        public double PressureRangeMin { get; set; } = default!;
        public double PressureRangeMax { get; set; } = default!;
        public double DefaultPressureRangeMin { get; set; } = default!;
        public double DefaultPressureRangeMax { get; set; } = default!;
        public bool TabletToolIsRelative { get; set; } = default!;
        public bool SupportsRotation { get; set; } = default!;
        public uint Rotation { get; set; } = default!;
        public uint DefaultRotation { get; set; } = default!;
        public bool IsVirtual { get; set; } = default!;
    }
    partial class InputDevice : KWinObject
    {
        private const string __Interface = "org.kde.KWin.InputDevice";
        public InputDevice(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task SetOutputNameAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("outputName");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetEnabledAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("enabled");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetCalibrationMatrixAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("calibrationMatrix");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetOrientationDBusAsync(int value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("orientationDBus");
                writer.WriteSignature("i");
                writer.WriteInt32(value);
                return writer.CreateMessage();
            }
        }
        public Task SetLeftHandedAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("leftHanded");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetDisableEventsOnExternalMouseAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("disableEventsOnExternalMouse");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetDisableWhileTypingAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("disableWhileTyping");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetPointerAccelerationAsync(double value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("pointerAcceleration");
                writer.WriteSignature("d");
                writer.WriteDouble(value);
                return writer.CreateMessage();
            }
        }
        public Task SetPointerAccelerationProfileFlatAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("pointerAccelerationProfileFlat");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetPointerAccelerationProfileAdaptiveAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("pointerAccelerationProfileAdaptive");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetTapToClickAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("tapToClick");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetLmrTapButtonMapAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("lmrTapButtonMap");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetTapAndDragAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("tapAndDrag");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetTapDragLockAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("tapDragLock");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetMiddleEmulationAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("middleEmulation");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetNaturalScrollAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("naturalScroll");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetScrollTwoFingerAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("scrollTwoFinger");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetScrollEdgeAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("scrollEdge");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetScrollOnButtonDownAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("scrollOnButtonDown");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetScrollButtonAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("scrollButton");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        public Task SetScrollFactorAsync(double value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("scrollFactor");
                writer.WriteSignature("d");
                writer.WriteDouble(value);
                return writer.CreateMessage();
            }
        }
        public Task SetClickMethodAreasAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("clickMethodAreas");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetClickMethodClickfingerAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("clickMethodClickfinger");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetOutputAreaAsync((double, double, double, double) value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("outputArea");
                writer.WriteSignature("(dddd)");
                WriteType_rddddz(ref writer, value);
                return writer.CreateMessage();
            }
        }
        public Task SetMapToWorkspaceAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("mapToWorkspace");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetPressureCurveAsync(string value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("pressureCurve");
                writer.WriteSignature("s");
                writer.WriteString(value);
                return writer.CreateMessage();
            }
        }
        public Task SetInputAreaAsync((double, double, double, double) value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("inputArea");
                writer.WriteSignature("(dddd)");
                WriteType_rddddz(ref writer, value);
                return writer.CreateMessage();
            }
        }
        public Task SetPressureRangeMinAsync(double value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("pressureRangeMin");
                writer.WriteSignature("d");
                writer.WriteDouble(value);
                return writer.CreateMessage();
            }
        }
        public Task SetPressureRangeMaxAsync(double value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("pressureRangeMax");
                writer.WriteSignature("d");
                writer.WriteDouble(value);
                return writer.CreateMessage();
            }
        }
        public Task SetTabletToolIsRelativeAsync(bool value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("tabletToolIsRelative");
                writer.WriteSignature("b");
                writer.WriteBool(value);
                return writer.CreateMessage();
            }
        }
        public Task SetRotationAsync(uint value)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: "org.freedesktop.DBus.Properties",
                    signature: "ssv",
                    member: "Set");
                writer.WriteString(__Interface);
                writer.WriteString("rotation");
                writer.WriteSignature("u");
                writer.WriteUInt32(value);
                return writer.CreateMessage();
            }
        }
        public Task<bool> GetKeyboardAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "keyboard"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetAlphaNumericKeyboardAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "alphaNumericKeyboard"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetPointerAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "pointer"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTouchpadAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "touchpad"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTouchAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "touch"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTabletToolAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletTool"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTabletPadAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletPad"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetGestureSupportAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "gestureSupport"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<string> GetNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "name"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<string> GetSysNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "sysName"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<string> GetOutputNameAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "outputName"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<(double, double)> GetSizeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "size"), (Message m, object? s) => ReadMessage_v_rddz(m, (KWinObject)s!), this);
        public Task<uint> GetProductAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "product"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<uint> GetVendorAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "vendor"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsDisableEventsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsDisableEvents"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "enabled"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "enabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<int> GetSupportedButtonsAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportedButtons"), (Message m, object? s) => ReadMessage_v_i(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsCalibrationMatrixAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsCalibrationMatrix"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<string> GetDefaultCalibrationMatrixAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultCalibrationMatrix"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<string> GetCalibrationMatrixAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "calibrationMatrix"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<int> GetOrientationDBusAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "orientationDBus"), (Message m, object? s) => ReadMessage_v_i(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsLeftHandedAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsLeftHanded"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetLeftHandedEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "leftHandedEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetLeftHandedAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "leftHanded"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsDisableEventsOnExternalMouseAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsDisableEventsOnExternalMouse"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetDisableEventsOnExternalMouseEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "disableEventsOnExternalMouseEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetDisableEventsOnExternalMouseAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "disableEventsOnExternalMouse"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsDisableWhileTypingAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsDisableWhileTyping"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetDisableWhileTypingEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "disableWhileTypingEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetDisableWhileTypingAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "disableWhileTyping"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsPointerAccelerationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsPointerAcceleration"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<double> GetDefaultPointerAccelerationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultPointerAcceleration"), (Message m, object? s) => ReadMessage_v_d(m, (KWinObject)s!), this);
        public Task<double> GetPointerAccelerationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "pointerAcceleration"), (Message m, object? s) => ReadMessage_v_d(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsPointerAccelerationProfileFlatAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsPointerAccelerationProfileFlat"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetDefaultPointerAccelerationProfileFlatAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultPointerAccelerationProfileFlat"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetPointerAccelerationProfileFlatAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "pointerAccelerationProfileFlat"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsPointerAccelerationProfileAdaptiveAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsPointerAccelerationProfileAdaptive"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetDefaultPointerAccelerationProfileAdaptiveAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultPointerAccelerationProfileAdaptive"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetPointerAccelerationProfileAdaptiveAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "pointerAccelerationProfileAdaptive"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<int> GetTapFingerCountAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tapFingerCount"), (Message m, object? s) => ReadMessage_v_i(m, (KWinObject)s!), this);
        public Task<bool> GetTapToClickEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tapToClickEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTapToClickAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tapToClick"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsLmrTapButtonMapAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsLmrTapButtonMap"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetLmrTapButtonMapEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "lmrTapButtonMapEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetLmrTapButtonMapAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "lmrTapButtonMap"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTapAndDragEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tapAndDragEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTapAndDragAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tapAndDrag"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTapDragLockEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tapDragLockEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTapDragLockAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tapDragLock"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsMiddleEmulationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsMiddleEmulation"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetMiddleEmulationEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "middleEmulationEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetMiddleEmulationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "middleEmulation"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsNaturalScrollAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsNaturalScroll"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetNaturalScrollEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "naturalScrollEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetNaturalScrollAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "naturalScroll"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsScrollTwoFingerAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsScrollTwoFinger"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetScrollTwoFingerEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scrollTwoFingerEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetScrollTwoFingerAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scrollTwoFinger"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsScrollEdgeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsScrollEdge"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetScrollEdgeEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scrollEdgeEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetScrollEdgeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scrollEdge"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsScrollOnButtonDownAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsScrollOnButtonDown"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetScrollOnButtonDownEnabledByDefaultAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scrollOnButtonDownEnabledByDefault"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<uint> GetDefaultScrollButtonAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultScrollButton"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<bool> GetScrollOnButtonDownAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scrollOnButtonDown"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<uint> GetScrollButtonAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scrollButton"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<double> GetScrollFactorAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scrollFactor"), (Message m, object? s) => ReadMessage_v_d(m, (KWinObject)s!), this);
        public Task<bool> GetSwitchDeviceAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "switchDevice"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetLidSwitchAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "lidSwitch"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetTabletModeSwitchAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletModeSwitch"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsClickMethodAreasAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsClickMethodAreas"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetDefaultClickMethodAreasAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultClickMethodAreas"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetClickMethodAreasAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "clickMethodAreas"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsClickMethodClickfingerAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsClickMethodClickfinger"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetDefaultClickMethodClickfingerAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultClickMethodClickfinger"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetClickMethodClickfingerAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "clickMethodClickfinger"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsOutputAreaAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsOutputArea"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<(double, double, double, double)> GetDefaultOutputAreaAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultOutputArea"), (Message m, object? s) => ReadMessage_v_rddddz(m, (KWinObject)s!), this);
        public Task<(double, double, double, double)> GetOutputAreaAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "outputArea"), (Message m, object? s) => ReadMessage_v_rddddz(m, (KWinObject)s!), this);
        public Task<bool> GetDefaultMapToWorkspaceAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultMapToWorkspace"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetMapToWorkspaceAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "mapToWorkspace"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<string> GetDeviceGroupIdAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "deviceGroupId"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<string> GetDefaultPressureCurveAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultPressureCurve"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<string> GetPressureCurveAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "pressureCurve"), (Message m, object? s) => ReadMessage_v_s(m, (KWinObject)s!), this);
        public Task<uint> GetTabletPadButtonCountAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletPadButtonCount"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<uint> GetTabletPadDialCountAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletPadDialCount"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<uint> GetTabletPadRingCountAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletPadRingCount"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<uint> GetTabletPadStripCountAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletPadStripCount"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsInputAreaAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsInputArea"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<(double, double, double, double)> GetDefaultInputAreaAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultInputArea"), (Message m, object? s) => ReadMessage_v_rddddz(m, (KWinObject)s!), this);
        public Task<(double, double, double, double)> GetInputAreaAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "inputArea"), (Message m, object? s) => ReadMessage_v_rddddz(m, (KWinObject)s!), this);
        public Task<uint[]> GetNumModesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "numModes"), (Message m, object? s) => ReadMessage_v_au(m, (KWinObject)s!), this);
        public Task<uint[]> GetCurrentModesAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "currentModes"), (Message m, object? s) => ReadMessage_v_au(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsPressureRangeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsPressureRange"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<double> GetPressureRangeMinAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "pressureRangeMin"), (Message m, object? s) => ReadMessage_v_d(m, (KWinObject)s!), this);
        public Task<double> GetPressureRangeMaxAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "pressureRangeMax"), (Message m, object? s) => ReadMessage_v_d(m, (KWinObject)s!), this);
        public Task<double> GetDefaultPressureRangeMinAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultPressureRangeMin"), (Message m, object? s) => ReadMessage_v_d(m, (KWinObject)s!), this);
        public Task<double> GetDefaultPressureRangeMaxAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultPressureRangeMax"), (Message m, object? s) => ReadMessage_v_d(m, (KWinObject)s!), this);
        public Task<bool> GetTabletToolIsRelativeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "tabletToolIsRelative"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetSupportsRotationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "supportsRotation"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<uint> GetRotationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "rotation"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<uint> GetDefaultRotationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "defaultRotation"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<bool> GetIsVirtualAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "isVirtual"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<InputDeviceProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static InputDeviceProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<InputDeviceProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<InputDeviceProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<InputDeviceProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "keyboard": invalidated.Add("Keyboard"); break;
                        case "alphaNumericKeyboard": invalidated.Add("AlphaNumericKeyboard"); break;
                        case "pointer": invalidated.Add("Pointer"); break;
                        case "touchpad": invalidated.Add("Touchpad"); break;
                        case "touch": invalidated.Add("Touch"); break;
                        case "tabletTool": invalidated.Add("TabletTool"); break;
                        case "tabletPad": invalidated.Add("TabletPad"); break;
                        case "gestureSupport": invalidated.Add("GestureSupport"); break;
                        case "name": invalidated.Add("Name"); break;
                        case "sysName": invalidated.Add("SysName"); break;
                        case "outputName": invalidated.Add("OutputName"); break;
                        case "size": invalidated.Add("Size"); break;
                        case "product": invalidated.Add("Product"); break;
                        case "vendor": invalidated.Add("Vendor"); break;
                        case "supportsDisableEvents": invalidated.Add("SupportsDisableEvents"); break;
                        case "enabled": invalidated.Add("Enabled"); break;
                        case "enabledByDefault": invalidated.Add("EnabledByDefault"); break;
                        case "supportedButtons": invalidated.Add("SupportedButtons"); break;
                        case "supportsCalibrationMatrix": invalidated.Add("SupportsCalibrationMatrix"); break;
                        case "defaultCalibrationMatrix": invalidated.Add("DefaultCalibrationMatrix"); break;
                        case "calibrationMatrix": invalidated.Add("CalibrationMatrix"); break;
                        case "orientationDBus": invalidated.Add("OrientationDBus"); break;
                        case "supportsLeftHanded": invalidated.Add("SupportsLeftHanded"); break;
                        case "leftHandedEnabledByDefault": invalidated.Add("LeftHandedEnabledByDefault"); break;
                        case "leftHanded": invalidated.Add("LeftHanded"); break;
                        case "supportsDisableEventsOnExternalMouse": invalidated.Add("SupportsDisableEventsOnExternalMouse"); break;
                        case "disableEventsOnExternalMouseEnabledByDefault": invalidated.Add("DisableEventsOnExternalMouseEnabledByDefault"); break;
                        case "disableEventsOnExternalMouse": invalidated.Add("DisableEventsOnExternalMouse"); break;
                        case "supportsDisableWhileTyping": invalidated.Add("SupportsDisableWhileTyping"); break;
                        case "disableWhileTypingEnabledByDefault": invalidated.Add("DisableWhileTypingEnabledByDefault"); break;
                        case "disableWhileTyping": invalidated.Add("DisableWhileTyping"); break;
                        case "supportsPointerAcceleration": invalidated.Add("SupportsPointerAcceleration"); break;
                        case "defaultPointerAcceleration": invalidated.Add("DefaultPointerAcceleration"); break;
                        case "pointerAcceleration": invalidated.Add("PointerAcceleration"); break;
                        case "supportsPointerAccelerationProfileFlat": invalidated.Add("SupportsPointerAccelerationProfileFlat"); break;
                        case "defaultPointerAccelerationProfileFlat": invalidated.Add("DefaultPointerAccelerationProfileFlat"); break;
                        case "pointerAccelerationProfileFlat": invalidated.Add("PointerAccelerationProfileFlat"); break;
                        case "supportsPointerAccelerationProfileAdaptive": invalidated.Add("SupportsPointerAccelerationProfileAdaptive"); break;
                        case "defaultPointerAccelerationProfileAdaptive": invalidated.Add("DefaultPointerAccelerationProfileAdaptive"); break;
                        case "pointerAccelerationProfileAdaptive": invalidated.Add("PointerAccelerationProfileAdaptive"); break;
                        case "tapFingerCount": invalidated.Add("TapFingerCount"); break;
                        case "tapToClickEnabledByDefault": invalidated.Add("TapToClickEnabledByDefault"); break;
                        case "tapToClick": invalidated.Add("TapToClick"); break;
                        case "supportsLmrTapButtonMap": invalidated.Add("SupportsLmrTapButtonMap"); break;
                        case "lmrTapButtonMapEnabledByDefault": invalidated.Add("LmrTapButtonMapEnabledByDefault"); break;
                        case "lmrTapButtonMap": invalidated.Add("LmrTapButtonMap"); break;
                        case "tapAndDragEnabledByDefault": invalidated.Add("TapAndDragEnabledByDefault"); break;
                        case "tapAndDrag": invalidated.Add("TapAndDrag"); break;
                        case "tapDragLockEnabledByDefault": invalidated.Add("TapDragLockEnabledByDefault"); break;
                        case "tapDragLock": invalidated.Add("TapDragLock"); break;
                        case "supportsMiddleEmulation": invalidated.Add("SupportsMiddleEmulation"); break;
                        case "middleEmulationEnabledByDefault": invalidated.Add("MiddleEmulationEnabledByDefault"); break;
                        case "middleEmulation": invalidated.Add("MiddleEmulation"); break;
                        case "supportsNaturalScroll": invalidated.Add("SupportsNaturalScroll"); break;
                        case "naturalScrollEnabledByDefault": invalidated.Add("NaturalScrollEnabledByDefault"); break;
                        case "naturalScroll": invalidated.Add("NaturalScroll"); break;
                        case "supportsScrollTwoFinger": invalidated.Add("SupportsScrollTwoFinger"); break;
                        case "scrollTwoFingerEnabledByDefault": invalidated.Add("ScrollTwoFingerEnabledByDefault"); break;
                        case "scrollTwoFinger": invalidated.Add("ScrollTwoFinger"); break;
                        case "supportsScrollEdge": invalidated.Add("SupportsScrollEdge"); break;
                        case "scrollEdgeEnabledByDefault": invalidated.Add("ScrollEdgeEnabledByDefault"); break;
                        case "scrollEdge": invalidated.Add("ScrollEdge"); break;
                        case "supportsScrollOnButtonDown": invalidated.Add("SupportsScrollOnButtonDown"); break;
                        case "scrollOnButtonDownEnabledByDefault": invalidated.Add("ScrollOnButtonDownEnabledByDefault"); break;
                        case "defaultScrollButton": invalidated.Add("DefaultScrollButton"); break;
                        case "scrollOnButtonDown": invalidated.Add("ScrollOnButtonDown"); break;
                        case "scrollButton": invalidated.Add("ScrollButton"); break;
                        case "scrollFactor": invalidated.Add("ScrollFactor"); break;
                        case "switchDevice": invalidated.Add("SwitchDevice"); break;
                        case "lidSwitch": invalidated.Add("LidSwitch"); break;
                        case "tabletModeSwitch": invalidated.Add("TabletModeSwitch"); break;
                        case "supportsClickMethodAreas": invalidated.Add("SupportsClickMethodAreas"); break;
                        case "defaultClickMethodAreas": invalidated.Add("DefaultClickMethodAreas"); break;
                        case "clickMethodAreas": invalidated.Add("ClickMethodAreas"); break;
                        case "supportsClickMethodClickfinger": invalidated.Add("SupportsClickMethodClickfinger"); break;
                        case "defaultClickMethodClickfinger": invalidated.Add("DefaultClickMethodClickfinger"); break;
                        case "clickMethodClickfinger": invalidated.Add("ClickMethodClickfinger"); break;
                        case "supportsOutputArea": invalidated.Add("SupportsOutputArea"); break;
                        case "defaultOutputArea": invalidated.Add("DefaultOutputArea"); break;
                        case "outputArea": invalidated.Add("OutputArea"); break;
                        case "defaultMapToWorkspace": invalidated.Add("DefaultMapToWorkspace"); break;
                        case "mapToWorkspace": invalidated.Add("MapToWorkspace"); break;
                        case "deviceGroupId": invalidated.Add("DeviceGroupId"); break;
                        case "defaultPressureCurve": invalidated.Add("DefaultPressureCurve"); break;
                        case "pressureCurve": invalidated.Add("PressureCurve"); break;
                        case "tabletPadButtonCount": invalidated.Add("TabletPadButtonCount"); break;
                        case "tabletPadDialCount": invalidated.Add("TabletPadDialCount"); break;
                        case "tabletPadRingCount": invalidated.Add("TabletPadRingCount"); break;
                        case "tabletPadStripCount": invalidated.Add("TabletPadStripCount"); break;
                        case "supportsInputArea": invalidated.Add("SupportsInputArea"); break;
                        case "defaultInputArea": invalidated.Add("DefaultInputArea"); break;
                        case "inputArea": invalidated.Add("InputArea"); break;
                        case "numModes": invalidated.Add("NumModes"); break;
                        case "currentModes": invalidated.Add("CurrentModes"); break;
                        case "supportsPressureRange": invalidated.Add("SupportsPressureRange"); break;
                        case "pressureRangeMin": invalidated.Add("PressureRangeMin"); break;
                        case "pressureRangeMax": invalidated.Add("PressureRangeMax"); break;
                        case "defaultPressureRangeMin": invalidated.Add("DefaultPressureRangeMin"); break;
                        case "defaultPressureRangeMax": invalidated.Add("DefaultPressureRangeMax"); break;
                        case "tabletToolIsRelative": invalidated.Add("TabletToolIsRelative"); break;
                        case "supportsRotation": invalidated.Add("SupportsRotation"); break;
                        case "rotation": invalidated.Add("Rotation"); break;
                        case "defaultRotation": invalidated.Add("DefaultRotation"); break;
                        case "isVirtual": invalidated.Add("IsVirtual"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static InputDeviceProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new InputDeviceProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "keyboard":
                        reader.ReadSignature("b"u8);
                        props.Keyboard = reader.ReadBool();
                        changedList?.Add("Keyboard");
                        break;
                    case "alphaNumericKeyboard":
                        reader.ReadSignature("b"u8);
                        props.AlphaNumericKeyboard = reader.ReadBool();
                        changedList?.Add("AlphaNumericKeyboard");
                        break;
                    case "pointer":
                        reader.ReadSignature("b"u8);
                        props.Pointer = reader.ReadBool();
                        changedList?.Add("Pointer");
                        break;
                    case "touchpad":
                        reader.ReadSignature("b"u8);
                        props.Touchpad = reader.ReadBool();
                        changedList?.Add("Touchpad");
                        break;
                    case "touch":
                        reader.ReadSignature("b"u8);
                        props.Touch = reader.ReadBool();
                        changedList?.Add("Touch");
                        break;
                    case "tabletTool":
                        reader.ReadSignature("b"u8);
                        props.TabletTool = reader.ReadBool();
                        changedList?.Add("TabletTool");
                        break;
                    case "tabletPad":
                        reader.ReadSignature("b"u8);
                        props.TabletPad = reader.ReadBool();
                        changedList?.Add("TabletPad");
                        break;
                    case "gestureSupport":
                        reader.ReadSignature("b"u8);
                        props.GestureSupport = reader.ReadBool();
                        changedList?.Add("GestureSupport");
                        break;
                    case "name":
                        reader.ReadSignature("s"u8);
                        props.Name = reader.ReadString();
                        changedList?.Add("Name");
                        break;
                    case "sysName":
                        reader.ReadSignature("s"u8);
                        props.SysName = reader.ReadString();
                        changedList?.Add("SysName");
                        break;
                    case "outputName":
                        reader.ReadSignature("s"u8);
                        props.OutputName = reader.ReadString();
                        changedList?.Add("OutputName");
                        break;
                    case "size":
                        reader.ReadSignature("(dd)"u8);
                        props.Size = ReadType_rddz(ref reader);
                        changedList?.Add("Size");
                        break;
                    case "product":
                        reader.ReadSignature("u"u8);
                        props.Product = reader.ReadUInt32();
                        changedList?.Add("Product");
                        break;
                    case "vendor":
                        reader.ReadSignature("u"u8);
                        props.Vendor = reader.ReadUInt32();
                        changedList?.Add("Vendor");
                        break;
                    case "supportsDisableEvents":
                        reader.ReadSignature("b"u8);
                        props.SupportsDisableEvents = reader.ReadBool();
                        changedList?.Add("SupportsDisableEvents");
                        break;
                    case "enabled":
                        reader.ReadSignature("b"u8);
                        props.Enabled = reader.ReadBool();
                        changedList?.Add("Enabled");
                        break;
                    case "enabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.EnabledByDefault = reader.ReadBool();
                        changedList?.Add("EnabledByDefault");
                        break;
                    case "supportedButtons":
                        reader.ReadSignature("i"u8);
                        props.SupportedButtons = reader.ReadInt32();
                        changedList?.Add("SupportedButtons");
                        break;
                    case "supportsCalibrationMatrix":
                        reader.ReadSignature("b"u8);
                        props.SupportsCalibrationMatrix = reader.ReadBool();
                        changedList?.Add("SupportsCalibrationMatrix");
                        break;
                    case "defaultCalibrationMatrix":
                        reader.ReadSignature("s"u8);
                        props.DefaultCalibrationMatrix = reader.ReadString();
                        changedList?.Add("DefaultCalibrationMatrix");
                        break;
                    case "calibrationMatrix":
                        reader.ReadSignature("s"u8);
                        props.CalibrationMatrix = reader.ReadString();
                        changedList?.Add("CalibrationMatrix");
                        break;
                    case "orientationDBus":
                        reader.ReadSignature("i"u8);
                        props.OrientationDBus = reader.ReadInt32();
                        changedList?.Add("OrientationDBus");
                        break;
                    case "supportsLeftHanded":
                        reader.ReadSignature("b"u8);
                        props.SupportsLeftHanded = reader.ReadBool();
                        changedList?.Add("SupportsLeftHanded");
                        break;
                    case "leftHandedEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.LeftHandedEnabledByDefault = reader.ReadBool();
                        changedList?.Add("LeftHandedEnabledByDefault");
                        break;
                    case "leftHanded":
                        reader.ReadSignature("b"u8);
                        props.LeftHanded = reader.ReadBool();
                        changedList?.Add("LeftHanded");
                        break;
                    case "supportsDisableEventsOnExternalMouse":
                        reader.ReadSignature("b"u8);
                        props.SupportsDisableEventsOnExternalMouse = reader.ReadBool();
                        changedList?.Add("SupportsDisableEventsOnExternalMouse");
                        break;
                    case "disableEventsOnExternalMouseEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.DisableEventsOnExternalMouseEnabledByDefault = reader.ReadBool();
                        changedList?.Add("DisableEventsOnExternalMouseEnabledByDefault");
                        break;
                    case "disableEventsOnExternalMouse":
                        reader.ReadSignature("b"u8);
                        props.DisableEventsOnExternalMouse = reader.ReadBool();
                        changedList?.Add("DisableEventsOnExternalMouse");
                        break;
                    case "supportsDisableWhileTyping":
                        reader.ReadSignature("b"u8);
                        props.SupportsDisableWhileTyping = reader.ReadBool();
                        changedList?.Add("SupportsDisableWhileTyping");
                        break;
                    case "disableWhileTypingEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.DisableWhileTypingEnabledByDefault = reader.ReadBool();
                        changedList?.Add("DisableWhileTypingEnabledByDefault");
                        break;
                    case "disableWhileTyping":
                        reader.ReadSignature("b"u8);
                        props.DisableWhileTyping = reader.ReadBool();
                        changedList?.Add("DisableWhileTyping");
                        break;
                    case "supportsPointerAcceleration":
                        reader.ReadSignature("b"u8);
                        props.SupportsPointerAcceleration = reader.ReadBool();
                        changedList?.Add("SupportsPointerAcceleration");
                        break;
                    case "defaultPointerAcceleration":
                        reader.ReadSignature("d"u8);
                        props.DefaultPointerAcceleration = reader.ReadDouble();
                        changedList?.Add("DefaultPointerAcceleration");
                        break;
                    case "pointerAcceleration":
                        reader.ReadSignature("d"u8);
                        props.PointerAcceleration = reader.ReadDouble();
                        changedList?.Add("PointerAcceleration");
                        break;
                    case "supportsPointerAccelerationProfileFlat":
                        reader.ReadSignature("b"u8);
                        props.SupportsPointerAccelerationProfileFlat = reader.ReadBool();
                        changedList?.Add("SupportsPointerAccelerationProfileFlat");
                        break;
                    case "defaultPointerAccelerationProfileFlat":
                        reader.ReadSignature("b"u8);
                        props.DefaultPointerAccelerationProfileFlat = reader.ReadBool();
                        changedList?.Add("DefaultPointerAccelerationProfileFlat");
                        break;
                    case "pointerAccelerationProfileFlat":
                        reader.ReadSignature("b"u8);
                        props.PointerAccelerationProfileFlat = reader.ReadBool();
                        changedList?.Add("PointerAccelerationProfileFlat");
                        break;
                    case "supportsPointerAccelerationProfileAdaptive":
                        reader.ReadSignature("b"u8);
                        props.SupportsPointerAccelerationProfileAdaptive = reader.ReadBool();
                        changedList?.Add("SupportsPointerAccelerationProfileAdaptive");
                        break;
                    case "defaultPointerAccelerationProfileAdaptive":
                        reader.ReadSignature("b"u8);
                        props.DefaultPointerAccelerationProfileAdaptive = reader.ReadBool();
                        changedList?.Add("DefaultPointerAccelerationProfileAdaptive");
                        break;
                    case "pointerAccelerationProfileAdaptive":
                        reader.ReadSignature("b"u8);
                        props.PointerAccelerationProfileAdaptive = reader.ReadBool();
                        changedList?.Add("PointerAccelerationProfileAdaptive");
                        break;
                    case "tapFingerCount":
                        reader.ReadSignature("i"u8);
                        props.TapFingerCount = reader.ReadInt32();
                        changedList?.Add("TapFingerCount");
                        break;
                    case "tapToClickEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.TapToClickEnabledByDefault = reader.ReadBool();
                        changedList?.Add("TapToClickEnabledByDefault");
                        break;
                    case "tapToClick":
                        reader.ReadSignature("b"u8);
                        props.TapToClick = reader.ReadBool();
                        changedList?.Add("TapToClick");
                        break;
                    case "supportsLmrTapButtonMap":
                        reader.ReadSignature("b"u8);
                        props.SupportsLmrTapButtonMap = reader.ReadBool();
                        changedList?.Add("SupportsLmrTapButtonMap");
                        break;
                    case "lmrTapButtonMapEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.LmrTapButtonMapEnabledByDefault = reader.ReadBool();
                        changedList?.Add("LmrTapButtonMapEnabledByDefault");
                        break;
                    case "lmrTapButtonMap":
                        reader.ReadSignature("b"u8);
                        props.LmrTapButtonMap = reader.ReadBool();
                        changedList?.Add("LmrTapButtonMap");
                        break;
                    case "tapAndDragEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.TapAndDragEnabledByDefault = reader.ReadBool();
                        changedList?.Add("TapAndDragEnabledByDefault");
                        break;
                    case "tapAndDrag":
                        reader.ReadSignature("b"u8);
                        props.TapAndDrag = reader.ReadBool();
                        changedList?.Add("TapAndDrag");
                        break;
                    case "tapDragLockEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.TapDragLockEnabledByDefault = reader.ReadBool();
                        changedList?.Add("TapDragLockEnabledByDefault");
                        break;
                    case "tapDragLock":
                        reader.ReadSignature("b"u8);
                        props.TapDragLock = reader.ReadBool();
                        changedList?.Add("TapDragLock");
                        break;
                    case "supportsMiddleEmulation":
                        reader.ReadSignature("b"u8);
                        props.SupportsMiddleEmulation = reader.ReadBool();
                        changedList?.Add("SupportsMiddleEmulation");
                        break;
                    case "middleEmulationEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.MiddleEmulationEnabledByDefault = reader.ReadBool();
                        changedList?.Add("MiddleEmulationEnabledByDefault");
                        break;
                    case "middleEmulation":
                        reader.ReadSignature("b"u8);
                        props.MiddleEmulation = reader.ReadBool();
                        changedList?.Add("MiddleEmulation");
                        break;
                    case "supportsNaturalScroll":
                        reader.ReadSignature("b"u8);
                        props.SupportsNaturalScroll = reader.ReadBool();
                        changedList?.Add("SupportsNaturalScroll");
                        break;
                    case "naturalScrollEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.NaturalScrollEnabledByDefault = reader.ReadBool();
                        changedList?.Add("NaturalScrollEnabledByDefault");
                        break;
                    case "naturalScroll":
                        reader.ReadSignature("b"u8);
                        props.NaturalScroll = reader.ReadBool();
                        changedList?.Add("NaturalScroll");
                        break;
                    case "supportsScrollTwoFinger":
                        reader.ReadSignature("b"u8);
                        props.SupportsScrollTwoFinger = reader.ReadBool();
                        changedList?.Add("SupportsScrollTwoFinger");
                        break;
                    case "scrollTwoFingerEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.ScrollTwoFingerEnabledByDefault = reader.ReadBool();
                        changedList?.Add("ScrollTwoFingerEnabledByDefault");
                        break;
                    case "scrollTwoFinger":
                        reader.ReadSignature("b"u8);
                        props.ScrollTwoFinger = reader.ReadBool();
                        changedList?.Add("ScrollTwoFinger");
                        break;
                    case "supportsScrollEdge":
                        reader.ReadSignature("b"u8);
                        props.SupportsScrollEdge = reader.ReadBool();
                        changedList?.Add("SupportsScrollEdge");
                        break;
                    case "scrollEdgeEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.ScrollEdgeEnabledByDefault = reader.ReadBool();
                        changedList?.Add("ScrollEdgeEnabledByDefault");
                        break;
                    case "scrollEdge":
                        reader.ReadSignature("b"u8);
                        props.ScrollEdge = reader.ReadBool();
                        changedList?.Add("ScrollEdge");
                        break;
                    case "supportsScrollOnButtonDown":
                        reader.ReadSignature("b"u8);
                        props.SupportsScrollOnButtonDown = reader.ReadBool();
                        changedList?.Add("SupportsScrollOnButtonDown");
                        break;
                    case "scrollOnButtonDownEnabledByDefault":
                        reader.ReadSignature("b"u8);
                        props.ScrollOnButtonDownEnabledByDefault = reader.ReadBool();
                        changedList?.Add("ScrollOnButtonDownEnabledByDefault");
                        break;
                    case "defaultScrollButton":
                        reader.ReadSignature("u"u8);
                        props.DefaultScrollButton = reader.ReadUInt32();
                        changedList?.Add("DefaultScrollButton");
                        break;
                    case "scrollOnButtonDown":
                        reader.ReadSignature("b"u8);
                        props.ScrollOnButtonDown = reader.ReadBool();
                        changedList?.Add("ScrollOnButtonDown");
                        break;
                    case "scrollButton":
                        reader.ReadSignature("u"u8);
                        props.ScrollButton = reader.ReadUInt32();
                        changedList?.Add("ScrollButton");
                        break;
                    case "scrollFactor":
                        reader.ReadSignature("d"u8);
                        props.ScrollFactor = reader.ReadDouble();
                        changedList?.Add("ScrollFactor");
                        break;
                    case "switchDevice":
                        reader.ReadSignature("b"u8);
                        props.SwitchDevice = reader.ReadBool();
                        changedList?.Add("SwitchDevice");
                        break;
                    case "lidSwitch":
                        reader.ReadSignature("b"u8);
                        props.LidSwitch = reader.ReadBool();
                        changedList?.Add("LidSwitch");
                        break;
                    case "tabletModeSwitch":
                        reader.ReadSignature("b"u8);
                        props.TabletModeSwitch = reader.ReadBool();
                        changedList?.Add("TabletModeSwitch");
                        break;
                    case "supportsClickMethodAreas":
                        reader.ReadSignature("b"u8);
                        props.SupportsClickMethodAreas = reader.ReadBool();
                        changedList?.Add("SupportsClickMethodAreas");
                        break;
                    case "defaultClickMethodAreas":
                        reader.ReadSignature("b"u8);
                        props.DefaultClickMethodAreas = reader.ReadBool();
                        changedList?.Add("DefaultClickMethodAreas");
                        break;
                    case "clickMethodAreas":
                        reader.ReadSignature("b"u8);
                        props.ClickMethodAreas = reader.ReadBool();
                        changedList?.Add("ClickMethodAreas");
                        break;
                    case "supportsClickMethodClickfinger":
                        reader.ReadSignature("b"u8);
                        props.SupportsClickMethodClickfinger = reader.ReadBool();
                        changedList?.Add("SupportsClickMethodClickfinger");
                        break;
                    case "defaultClickMethodClickfinger":
                        reader.ReadSignature("b"u8);
                        props.DefaultClickMethodClickfinger = reader.ReadBool();
                        changedList?.Add("DefaultClickMethodClickfinger");
                        break;
                    case "clickMethodClickfinger":
                        reader.ReadSignature("b"u8);
                        props.ClickMethodClickfinger = reader.ReadBool();
                        changedList?.Add("ClickMethodClickfinger");
                        break;
                    case "supportsOutputArea":
                        reader.ReadSignature("b"u8);
                        props.SupportsOutputArea = reader.ReadBool();
                        changedList?.Add("SupportsOutputArea");
                        break;
                    case "defaultOutputArea":
                        reader.ReadSignature("(dddd)"u8);
                        props.DefaultOutputArea = ReadType_rddddz(ref reader);
                        changedList?.Add("DefaultOutputArea");
                        break;
                    case "outputArea":
                        reader.ReadSignature("(dddd)"u8);
                        props.OutputArea = ReadType_rddddz(ref reader);
                        changedList?.Add("OutputArea");
                        break;
                    case "defaultMapToWorkspace":
                        reader.ReadSignature("b"u8);
                        props.DefaultMapToWorkspace = reader.ReadBool();
                        changedList?.Add("DefaultMapToWorkspace");
                        break;
                    case "mapToWorkspace":
                        reader.ReadSignature("b"u8);
                        props.MapToWorkspace = reader.ReadBool();
                        changedList?.Add("MapToWorkspace");
                        break;
                    case "deviceGroupId":
                        reader.ReadSignature("s"u8);
                        props.DeviceGroupId = reader.ReadString();
                        changedList?.Add("DeviceGroupId");
                        break;
                    case "defaultPressureCurve":
                        reader.ReadSignature("s"u8);
                        props.DefaultPressureCurve = reader.ReadString();
                        changedList?.Add("DefaultPressureCurve");
                        break;
                    case "pressureCurve":
                        reader.ReadSignature("s"u8);
                        props.PressureCurve = reader.ReadString();
                        changedList?.Add("PressureCurve");
                        break;
                    case "tabletPadButtonCount":
                        reader.ReadSignature("u"u8);
                        props.TabletPadButtonCount = reader.ReadUInt32();
                        changedList?.Add("TabletPadButtonCount");
                        break;
                    case "tabletPadDialCount":
                        reader.ReadSignature("u"u8);
                        props.TabletPadDialCount = reader.ReadUInt32();
                        changedList?.Add("TabletPadDialCount");
                        break;
                    case "tabletPadRingCount":
                        reader.ReadSignature("u"u8);
                        props.TabletPadRingCount = reader.ReadUInt32();
                        changedList?.Add("TabletPadRingCount");
                        break;
                    case "tabletPadStripCount":
                        reader.ReadSignature("u"u8);
                        props.TabletPadStripCount = reader.ReadUInt32();
                        changedList?.Add("TabletPadStripCount");
                        break;
                    case "supportsInputArea":
                        reader.ReadSignature("b"u8);
                        props.SupportsInputArea = reader.ReadBool();
                        changedList?.Add("SupportsInputArea");
                        break;
                    case "defaultInputArea":
                        reader.ReadSignature("(dddd)"u8);
                        props.DefaultInputArea = ReadType_rddddz(ref reader);
                        changedList?.Add("DefaultInputArea");
                        break;
                    case "inputArea":
                        reader.ReadSignature("(dddd)"u8);
                        props.InputArea = ReadType_rddddz(ref reader);
                        changedList?.Add("InputArea");
                        break;
                    case "numModes":
                        reader.ReadSignature("au"u8);
                        props.NumModes = reader.ReadArrayOfUInt32();
                        changedList?.Add("NumModes");
                        break;
                    case "currentModes":
                        reader.ReadSignature("au"u8);
                        props.CurrentModes = reader.ReadArrayOfUInt32();
                        changedList?.Add("CurrentModes");
                        break;
                    case "supportsPressureRange":
                        reader.ReadSignature("b"u8);
                        props.SupportsPressureRange = reader.ReadBool();
                        changedList?.Add("SupportsPressureRange");
                        break;
                    case "pressureRangeMin":
                        reader.ReadSignature("d"u8);
                        props.PressureRangeMin = reader.ReadDouble();
                        changedList?.Add("PressureRangeMin");
                        break;
                    case "pressureRangeMax":
                        reader.ReadSignature("d"u8);
                        props.PressureRangeMax = reader.ReadDouble();
                        changedList?.Add("PressureRangeMax");
                        break;
                    case "defaultPressureRangeMin":
                        reader.ReadSignature("d"u8);
                        props.DefaultPressureRangeMin = reader.ReadDouble();
                        changedList?.Add("DefaultPressureRangeMin");
                        break;
                    case "defaultPressureRangeMax":
                        reader.ReadSignature("d"u8);
                        props.DefaultPressureRangeMax = reader.ReadDouble();
                        changedList?.Add("DefaultPressureRangeMax");
                        break;
                    case "tabletToolIsRelative":
                        reader.ReadSignature("b"u8);
                        props.TabletToolIsRelative = reader.ReadBool();
                        changedList?.Add("TabletToolIsRelative");
                        break;
                    case "supportsRotation":
                        reader.ReadSignature("b"u8);
                        props.SupportsRotation = reader.ReadBool();
                        changedList?.Add("SupportsRotation");
                        break;
                    case "rotation":
                        reader.ReadSignature("u"u8);
                        props.Rotation = reader.ReadUInt32();
                        changedList?.Add("Rotation");
                        break;
                    case "defaultRotation":
                        reader.ReadSignature("u"u8);
                        props.DefaultRotation = reader.ReadUInt32();
                        changedList?.Add("DefaultRotation");
                        break;
                    case "isVirtual":
                        reader.ReadSignature("b"u8);
                        props.IsVirtual = reader.ReadBool();
                        changedList?.Add("IsVirtual");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record NightLightProperties
    {
        public bool Inhibited { get; set; } = default!;
        public bool Enabled { get; set; } = default!;
        public bool Running { get; set; } = default!;
        public bool Available { get; set; } = default!;
        public uint CurrentTemperature { get; set; } = default!;
        public uint TargetTemperature { get; set; } = default!;
        public uint Mode { get; set; } = default!;
        public bool Daylight { get; set; } = default!;
        public ulong PreviousTransitionDateTime { get; set; } = default!;
        public uint PreviousTransitionDuration { get; set; } = default!;
        public ulong ScheduledTransitionDateTime { get; set; } = default!;
        public uint ScheduledTransitionDuration { get; set; } = default!;
    }
    partial class NightLight : KWinObject
    {
        private const string __Interface = "org.kde.KWin.NightLight";
        public NightLight(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<uint> InhibitAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_u(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "inhibit");
                return writer.CreateMessage();
            }
        }
        public Task UninhibitAsync(uint cookie)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "uninhibit");
                writer.WriteUInt32(cookie);
                return writer.CreateMessage();
            }
        }
        public Task PreviewAsync(uint temperature)
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "u",
                    member: "preview");
                writer.WriteUInt32(temperature);
                return writer.CreateMessage();
            }
        }
        public Task StopPreviewAsync()
        {
            return this.Connection.CallMethodAsync(CreateMessage());
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    member: "stopPreview");
                return writer.CreateMessage();
            }
        }
        public Task<bool> GetInhibitedAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "inhibited"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetEnabledAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "enabled"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetRunningAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "running"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<bool> GetAvailableAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "available"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<uint> GetCurrentTemperatureAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "currentTemperature"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<uint> GetTargetTemperatureAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "targetTemperature"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<uint> GetModeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "mode"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<bool> GetDaylightAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "daylight"), (Message m, object? s) => ReadMessage_v_b(m, (KWinObject)s!), this);
        public Task<ulong> GetPreviousTransitionDateTimeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "previousTransitionDateTime"), (Message m, object? s) => ReadMessage_v_t(m, (KWinObject)s!), this);
        public Task<uint> GetPreviousTransitionDurationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "previousTransitionDuration"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<ulong> GetScheduledTransitionDateTimeAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scheduledTransitionDateTime"), (Message m, object? s) => ReadMessage_v_t(m, (KWinObject)s!), this);
        public Task<uint> GetScheduledTransitionDurationAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "scheduledTransitionDuration"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<NightLightProperties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static NightLightProperties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<NightLightProperties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<NightLightProperties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<NightLightProperties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "inhibited": invalidated.Add("Inhibited"); break;
                        case "enabled": invalidated.Add("Enabled"); break;
                        case "running": invalidated.Add("Running"); break;
                        case "available": invalidated.Add("Available"); break;
                        case "currentTemperature": invalidated.Add("CurrentTemperature"); break;
                        case "targetTemperature": invalidated.Add("TargetTemperature"); break;
                        case "mode": invalidated.Add("Mode"); break;
                        case "daylight": invalidated.Add("Daylight"); break;
                        case "previousTransitionDateTime": invalidated.Add("PreviousTransitionDateTime"); break;
                        case "previousTransitionDuration": invalidated.Add("PreviousTransitionDuration"); break;
                        case "scheduledTransitionDateTime": invalidated.Add("ScheduledTransitionDateTime"); break;
                        case "scheduledTransitionDuration": invalidated.Add("ScheduledTransitionDuration"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static NightLightProperties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new NightLightProperties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "inhibited":
                        reader.ReadSignature("b"u8);
                        props.Inhibited = reader.ReadBool();
                        changedList?.Add("Inhibited");
                        break;
                    case "enabled":
                        reader.ReadSignature("b"u8);
                        props.Enabled = reader.ReadBool();
                        changedList?.Add("Enabled");
                        break;
                    case "running":
                        reader.ReadSignature("b"u8);
                        props.Running = reader.ReadBool();
                        changedList?.Add("Running");
                        break;
                    case "available":
                        reader.ReadSignature("b"u8);
                        props.Available = reader.ReadBool();
                        changedList?.Add("Available");
                        break;
                    case "currentTemperature":
                        reader.ReadSignature("u"u8);
                        props.CurrentTemperature = reader.ReadUInt32();
                        changedList?.Add("CurrentTemperature");
                        break;
                    case "targetTemperature":
                        reader.ReadSignature("u"u8);
                        props.TargetTemperature = reader.ReadUInt32();
                        changedList?.Add("TargetTemperature");
                        break;
                    case "mode":
                        reader.ReadSignature("u"u8);
                        props.Mode = reader.ReadUInt32();
                        changedList?.Add("Mode");
                        break;
                    case "daylight":
                        reader.ReadSignature("b"u8);
                        props.Daylight = reader.ReadBool();
                        changedList?.Add("Daylight");
                        break;
                    case "previousTransitionDateTime":
                        reader.ReadSignature("t"u8);
                        props.PreviousTransitionDateTime = reader.ReadUInt64();
                        changedList?.Add("PreviousTransitionDateTime");
                        break;
                    case "previousTransitionDuration":
                        reader.ReadSignature("u"u8);
                        props.PreviousTransitionDuration = reader.ReadUInt32();
                        changedList?.Add("PreviousTransitionDuration");
                        break;
                    case "scheduledTransitionDateTime":
                        reader.ReadSignature("t"u8);
                        props.ScheduledTransitionDateTime = reader.ReadUInt64();
                        changedList?.Add("ScheduledTransitionDateTime");
                        break;
                    case "scheduledTransitionDuration":
                        reader.ReadSignature("u"u8);
                        props.ScheduledTransitionDuration = reader.ReadUInt32();
                        changedList?.Add("ScheduledTransitionDuration");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    record ScreenShot2Properties
    {
        public uint Version { get; set; } = default!;
    }
    partial class ScreenShot2 : KWinObject
    {
        private const string __Interface = "org.kde.KWin.ScreenShot2";
        public ScreenShot2(KWinService service, ObjectPath path) : base(service, path)
        { }
        public Task<Dictionary<string, VariantValue>> CaptureWindowAsync(string handle, Dictionary<string, VariantValue> options, System.Runtime.InteropServices.SafeHandle pipe)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "sa{sv}h",
                    member: "CaptureWindow");
                writer.WriteString(handle);
                writer.WriteDictionary(options);
                writer.WriteHandle(pipe);
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, VariantValue>> CaptureActiveWindowAsync(Dictionary<string, VariantValue> options, System.Runtime.InteropServices.SafeHandle pipe)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sv}h",
                    member: "CaptureActiveWindow");
                writer.WriteDictionary(options);
                writer.WriteHandle(pipe);
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, VariantValue>> CaptureAreaAsync(int x, int y, uint width, uint height, Dictionary<string, VariantValue> options, System.Runtime.InteropServices.SafeHandle pipe)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "iiuua{sv}h",
                    member: "CaptureArea");
                writer.WriteInt32(x);
                writer.WriteInt32(y);
                writer.WriteUInt32(width);
                writer.WriteUInt32(height);
                writer.WriteDictionary(options);
                writer.WriteHandle(pipe);
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, VariantValue>> CaptureScreenAsync(string name, Dictionary<string, VariantValue> options, System.Runtime.InteropServices.SafeHandle pipe)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "sa{sv}h",
                    member: "CaptureScreen");
                writer.WriteString(name);
                writer.WriteDictionary(options);
                writer.WriteHandle(pipe);
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, VariantValue>> CaptureActiveScreenAsync(Dictionary<string, VariantValue> options, System.Runtime.InteropServices.SafeHandle pipe)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sv}h",
                    member: "CaptureActiveScreen");
                writer.WriteDictionary(options);
                writer.WriteHandle(pipe);
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, VariantValue>> CaptureInteractiveAsync(uint kind, Dictionary<string, VariantValue> options, System.Runtime.InteropServices.SafeHandle pipe)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "ua{sv}h",
                    member: "CaptureInteractive");
                writer.WriteUInt32(kind);
                writer.WriteDictionary(options);
                writer.WriteHandle(pipe);
                return writer.CreateMessage();
            }
        }
        public Task<Dictionary<string, VariantValue>> CaptureWorkspaceAsync(Dictionary<string, VariantValue> options, System.Runtime.InteropServices.SafeHandle pipe)
        {
            return this.Connection.CallMethodAsync(CreateMessage(), (Message m, object? s) => ReadMessage_aesv(m, (KWinObject)s!), this);
            MessageBuffer CreateMessage()
            {
                var writer = this.Connection.GetMessageWriter();
                writer.WriteMethodCallHeader(
                    destination: Service.Destination,
                    path: Path,
                    @interface: __Interface,
                    signature: "a{sv}h",
                    member: "CaptureWorkspace");
                writer.WriteDictionary(options);
                writer.WriteHandle(pipe);
                return writer.CreateMessage();
            }
        }
        public Task<uint> GetVersionAsync()
            => this.Connection.CallMethodAsync(CreateGetPropertyMessage(__Interface, "Version"), (Message m, object? s) => ReadMessage_v_u(m, (KWinObject)s!), this);
        public Task<ScreenShot2Properties> GetPropertiesAsync()
        {
            return this.Connection.CallMethodAsync(CreateGetAllPropertiesMessage(__Interface), (Message m, object? s) => ReadMessage(m, (KWinObject)s!), this);
            static ScreenShot2Properties ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                return ReadProperties(ref reader);
            }
        }
        public ValueTask<IDisposable> WatchPropertiesChangedAsync(Action<Exception?, PropertyChanges<ScreenShot2Properties>> handler, bool emitOnCapturedContext = true, ObserverFlags flags = ObserverFlags.None)
        {
            return base.WatchPropertiesChangedAsync(__Interface, (Message m, object? s) => ReadMessage(m, (KWinObject)s!), handler, emitOnCapturedContext, flags);
            static PropertyChanges<ScreenShot2Properties> ReadMessage(Message message, KWinObject _)
            {
                var reader = message.GetBodyReader();
                reader.ReadString(); // interface
                List<string> changed = new();
                return new PropertyChanges<ScreenShot2Properties>(ReadProperties(ref reader, changed), ReadInvalidated(ref reader), changed.ToArray());
            }
            static string[] ReadInvalidated(ref Reader reader)
            {
                List<string>? invalidated = null;
                ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.String);
                while (reader.HasNext(arrayEnd))
                {
                    invalidated ??= new();
                    var property = reader.ReadString();
                    switch (property)
                    {
                        case "Version": invalidated.Add("Version"); break;
                    }
                }
                return invalidated?.ToArray() ?? Array.Empty<string>();
            }
        }
        private static ScreenShot2Properties ReadProperties(ref Reader reader, List<string>? changedList = null)
        {
            var props = new ScreenShot2Properties();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                var property = reader.ReadString();
                switch (property)
                {
                    case "Version":
                        reader.ReadSignature("u"u8);
                        props.Version = reader.ReadUInt32();
                        changedList?.Add("Version");
                        break;
                    default:
                        reader.ReadVariantValue();
                        break;
                }
            }
            return props;
        }
    }
    partial class KWinService
    {
        public Tmds.DBus.Protocol.Connection Connection { get; }
        public string Destination { get; }
        public KWinService(Tmds.DBus.Protocol.Connection connection, string destination)
            => (Connection, Destination) = (connection, destination);
        public ColorPicker CreateColorPicker(ObjectPath path) => new ColorPicker(this, path);
        public Compositing CreateCompositing(ObjectPath path) => new Compositing(this, path);
        public Effects CreateEffects(ObjectPath path) => new Effects(this, path);
        public FTrace CreateFTrace(ObjectPath path) => new FTrace(this, path);
        public KWin CreateKWin(ObjectPath path) => new KWin(this, path);
        public KeyboardLayouts CreateKeyboardLayouts(ObjectPath path) => new KeyboardLayouts(this, path);
        public Plugins CreatePlugins(ObjectPath path) => new Plugins(this, path);
        public ScreenSaver CreateScreenSaver(ObjectPath path) => new ScreenSaver(this, path);
        public Screensaver CreateScreensaver(ObjectPath path) => new Screensaver(this, path);
        public Scripting CreateScripting(ObjectPath path) => new Scripting(this, path);
        public Script CreateScript(ObjectPath path) => new Script(this, path);
        public Session CreateSession(ObjectPath path) => new Session(this, path);
        public VirtualDesktopManager CreateVirtualDesktopManager(ObjectPath path) => new VirtualDesktopManager(this, path);
        public VirtualKeyboard CreateVirtualKeyboard(ObjectPath path) => new VirtualKeyboard(this, path);
        public Krunner1 CreateKrunner1(ObjectPath path) => new Krunner1(this, path);
        public Component CreateComponent(ObjectPath path) => new Component(this, path);
        public KGlobalAccel CreateKGlobalAccel(ObjectPath path) => new KGlobalAccel(this, path);
        public Touchpad CreateTouchpad(ObjectPath path) => new Touchpad(this, path);
        public KeyboardMonitor CreateKeyboardMonitor(ObjectPath path) => new KeyboardMonitor(this, path);
        public TabletModeManager CreateTabletModeManager(ObjectPath path) => new TabletModeManager(this, path);
        public BlendChanges CreateBlendChanges(ObjectPath path) => new BlendChanges(this, path);
        public InputCaptureManager CreateInputCaptureManager(ObjectPath path) => new InputCaptureManager(this, path);
        public RemoteDesktop CreateRemoteDesktop(ObjectPath path) => new RemoteDesktop(this, path);
        public OutputLocator1 CreateOutputLocator1(ObjectPath path) => new OutputLocator1(this, path);
        public SystemBell1 CreateSystemBell1(ObjectPath path) => new SystemBell1(this, path);
        public WindowView1 CreateWindowView1(ObjectPath path) => new WindowView1(this, path);
        public HighlightWindow CreateHighlightWindow(ObjectPath path) => new HighlightWindow(this, path);
        public InputDeviceManager CreateInputDeviceManager(ObjectPath path) => new InputDeviceManager(this, path);
        public InputDevice CreateInputDevice(ObjectPath path) => new InputDevice(this, path);
        public NightLight CreateNightLight(ObjectPath path) => new NightLight(this, path);
        public ScreenShot2 CreateScreenShot2(ObjectPath path) => new ScreenShot2(this, path);
    }
    class KWinObject
    {
        public KWinService Service { get; }
        public ObjectPath Path { get; }
        protected Tmds.DBus.Protocol.Connection Connection => Service.Connection;
        protected KWinObject(KWinService service, ObjectPath path)
            => (Service, Path) = (service, path);
        protected MessageBuffer CreateGetPropertyMessage(string @interface, string property)
        {
            var writer = this.Connection.GetMessageWriter();
            writer.WriteMethodCallHeader(
                destination: Service.Destination,
                path: Path,
                @interface: "org.freedesktop.DBus.Properties",
                signature: "ss",
                member: "Get");
            writer.WriteString(@interface);
            writer.WriteString(property);
            return writer.CreateMessage();
        }
        protected MessageBuffer CreateGetAllPropertiesMessage(string @interface)
        {
            var writer = this.Connection.GetMessageWriter();
            writer.WriteMethodCallHeader(
                destination: Service.Destination,
                path: Path,
                @interface: "org.freedesktop.DBus.Properties",
                signature: "s",
                member: "GetAll");
            writer.WriteString(@interface);
            return writer.CreateMessage();
        }
        protected ValueTask<IDisposable> WatchPropertiesChangedAsync<TProperties>(string @interface, MessageValueReader<PropertyChanges<TProperties>> reader, Action<Exception?, PropertyChanges<TProperties>> handler, bool emitOnCapturedContext, ObserverFlags flags)
        {
            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Sender = Service.Destination,
                Path = Path,
                Interface = "org.freedesktop.DBus.Properties",
                Member = "PropertiesChanged",
                Arg0 = @interface
            };
            return this.Connection.AddMatchAsync(rule, reader,
                                                    (Exception? ex, PropertyChanges<TProperties> changes, object? rs, object? hs) => ((Action<Exception?, PropertyChanges<TProperties>>)hs!).Invoke(ex, changes),
                                                    this, handler, emitOnCapturedContext, flags);
        }
        public ValueTask<IDisposable> WatchSignalAsync<TArg>(string sender, string @interface, ObjectPath path, string signal, MessageValueReader<TArg> reader, Action<Exception?, TArg> handler, bool emitOnCapturedContext, ObserverFlags flags)
        {
            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Sender = sender,
                Path = path,
                Member = signal,
                Interface = @interface
            };
            return this.Connection.AddMatchAsync(rule, reader,
                                                    (Exception? ex, TArg arg, object? rs, object? hs) => ((Action<Exception?, TArg>)hs!).Invoke(ex, arg),
                                                    this, handler, emitOnCapturedContext, flags);
        }
        public ValueTask<IDisposable> WatchSignalAsync(string sender, string @interface, ObjectPath path, string signal, Action<Exception?> handler, bool emitOnCapturedContext, ObserverFlags flags)
        {
            var rule = new MatchRule
            {
                Type = MessageType.Signal,
                Sender = sender,
                Path = path,
                Member = signal,
                Interface = @interface
            };
            return this.Connection.AddMatchAsync<object>(rule, (Message message, object? state) => null!,
                                                            (Exception? ex, object v, object? rs, object? hs) => ((Action<Exception?>)hs!).Invoke(ex), this, handler, emitOnCapturedContext, flags);
        }
        protected static (uint) ReadMessage_ruz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_ruz(ref reader);
        }
        protected static bool ReadMessage_b(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadBool();
        }
        protected static bool ReadMessage_v_b(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("b"u8);
            return reader.ReadBool();
        }
        protected static string ReadMessage_v_s(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("s"u8);
            return reader.ReadString();
        }
        protected static string[] ReadMessage_v_as(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("as"u8);
            return reader.ReadArrayOfString();
        }
        protected static bool[] ReadMessage_ab(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadArrayOfBool();
        }
        protected static string ReadMessage_s(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadString();
        }
        protected static int ReadMessage_i(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadInt32();
        }
        protected static Dictionary<string, VariantValue> ReadMessage_aesv(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadDictionaryOfStringToVariantValue();
        }
        protected static uint ReadMessage_u(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadUInt32();
        }
        protected static (string, string, string)[] ReadMessage_arsssz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_arsssz(ref reader);
        }
        protected static (string, (int, string, string)) ReadMessage_srissz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadString();
            var arg1 = ReadType_rissz(ref reader);
            return (arg0, arg1);
        }
        protected static uint ReadMessage_v_u(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("u"u8);
            return reader.ReadUInt32();
        }
        protected static (int, string, string)[] ReadMessage_v_arissz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("a(iss)"u8);
            return ReadType_arissz(ref reader);
        }
        protected static (string, string, string, uint, double, Dictionary<string, VariantValue>)[] ReadMessage_arsssudaesvz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_arsssudaesvz(ref reader);
        }
        protected static string[] ReadMessage_as(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadArrayOfString();
        }
        protected static (string, string, string, string, string, string, int[], int[])[] ReadMessage_arssssssaiaiz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_arssssssaiaiz(ref reader);
        }
        protected static (string, string, long) ReadMessage_ssx(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadString();
            var arg1 = reader.ReadString();
            var arg2 = reader.ReadInt64();
            return (arg0, arg1, arg2);
        }
        protected static ObjectPath[] ReadMessage_ao(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadArrayOfObjectPath();
        }
        protected static string[][] ReadMessage_aas(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_aas(ref reader);
        }
        protected static int[] ReadMessage_ai(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadArrayOfInt32();
        }
        protected static (int[])[] ReadMessage_araiz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return ReadType_araiz(ref reader);
        }
        protected static ObjectPath ReadMessage_o(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            return reader.ReadObjectPath();
        }
        protected static (string[], int[]) ReadMessage_asai(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadArrayOfString();
            var arg1 = reader.ReadArrayOfInt32();
            return (arg0, arg1);
        }
        protected static (string[], (int[])[]) ReadMessage_asaraiz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadArrayOfString();
            var arg1 = ReadType_araiz(ref reader);
            return (arg0, arg1);
        }
        protected static (bool, uint, uint, uint, ushort) ReadMessage_buuuq(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadBool();
            var arg1 = reader.ReadUInt32();
            var arg2 = reader.ReadUInt32();
            var arg3 = reader.ReadUInt32();
            var arg4 = reader.ReadUInt16();
            return (arg0, arg1, arg2, arg3, arg4);
        }
        protected static (System.Runtime.InteropServices.SafeHandle, int) ReadMessage_hi(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            var arg0 = reader.ReadHandle<Microsoft.Win32.SafeHandles.SafeFileHandle>();
            var arg1 = reader.ReadInt32();
            return (arg0, arg1);
        }
        protected static (double, double) ReadMessage_v_rddz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("(dd)"u8);
            return ReadType_rddz(ref reader);
        }
        protected static int ReadMessage_v_i(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("i"u8);
            return reader.ReadInt32();
        }
        protected static double ReadMessage_v_d(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("d"u8);
            return reader.ReadDouble();
        }
        protected static (double, double, double, double) ReadMessage_v_rddddz(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("(dddd)"u8);
            return ReadType_rddddz(ref reader);
        }
        protected static uint[] ReadMessage_v_au(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("au"u8);
            return reader.ReadArrayOfUInt32();
        }
        protected static ulong ReadMessage_v_t(Message message, KWinObject _)
        {
            var reader = message.GetBodyReader();
            reader.ReadSignature("t"u8);
            return reader.ReadUInt64();
        }
        protected static (int, string, string)[] ReadType_arissz(ref Reader reader)
        {
            List<(int, string, string)> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(ReadType_rissz(ref reader));
            }
            return list.ToArray();
        }
        protected static (int, string, string) ReadType_rissz(ref Reader reader)
        {
            return (reader.ReadInt32(), reader.ReadString(), reader.ReadString());
        }
        protected static (double, double) ReadType_rddz(ref Reader reader)
        {
            return (reader.ReadDouble(), reader.ReadDouble());
        }
        protected static (double, double, double, double) ReadType_rddddz(ref Reader reader)
        {
            return (reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble(), reader.ReadDouble());
        }
        protected static (uint) ReadType_ruz(ref Reader reader)
        {
            return (reader.ReadUInt32());
        }
        protected static (string, string, string)[] ReadType_arsssz(ref Reader reader)
        {
            List<(string, string, string)> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(ReadType_rsssz(ref reader));
            }
            return list.ToArray();
        }
        protected static (string, string, string) ReadType_rsssz(ref Reader reader)
        {
            return (reader.ReadString(), reader.ReadString(), reader.ReadString());
        }
        protected static (string, string, string, uint, double, Dictionary<string, VariantValue>)[] ReadType_arsssudaesvz(ref Reader reader)
        {
            List<(string, string, string, uint, double, Dictionary<string, VariantValue>)> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(ReadType_rsssudaesvz(ref reader));
            }
            return list.ToArray();
        }
        protected static (string, string, string, uint, double, Dictionary<string, VariantValue>) ReadType_rsssudaesvz(ref Reader reader)
        {
            return (reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadUInt32(), reader.ReadDouble(), reader.ReadDictionaryOfStringToVariantValue());
        }
        protected static (string, string, string, string, string, string, int[], int[])[] ReadType_arssssssaiaiz(ref Reader reader)
        {
            List<(string, string, string, string, string, string, int[], int[])> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(ReadType_rssssssaiaiz(ref reader));
            }
            return list.ToArray();
        }
        protected static (string, string, string, string, string, string, int[], int[]) ReadType_rssssssaiaiz(ref Reader reader)
        {
            return (reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadArrayOfInt32(), reader.ReadArrayOfInt32());
        }
        protected static string[][] ReadType_aas(ref Reader reader)
        {
            List<string[]> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Array);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(reader.ReadArrayOfString());
            }
            return list.ToArray();
        }
        protected static (int[])[] ReadType_araiz(ref Reader reader)
        {
            List<(int[])> list = new();
            ArrayEnd arrayEnd = reader.ReadArrayStart(DBusType.Struct);
            while (reader.HasNext(arrayEnd))
            {
                list.Add(ReadType_raiz(ref reader));
            }
            return list.ToArray();
        }
        protected static (int[]) ReadType_raiz(ref Reader reader)
        {
            return (reader.ReadArrayOfInt32());
        }
        protected static void WriteType_raiz(ref MessageWriter writer, (int[]) value)
        {
            writer.WriteStructureStart();
            writer.WriteArray(value.Item1);
        }
        protected static void WriteType_araiz(ref MessageWriter writer, (int[])[] value)
        {
            ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Struct);
            foreach (var item in value)
            {
                WriteType_raiz(ref writer, item);
            }
            writer.WriteArrayEnd(arrayStart);
        }
        protected static void WriteType_riz(ref MessageWriter writer, (int) value)
        {
            writer.WriteStructureStart();
            writer.WriteInt32(value.Item1);
        }
        protected static void WriteType_aruuz(ref MessageWriter writer, (uint, uint)[] value)
        {
            ArrayStart arrayStart = writer.WriteArrayStart(DBusType.Struct);
            foreach (var item in value)
            {
                WriteType_ruuz(ref writer, item);
            }
            writer.WriteArrayEnd(arrayStart);
        }
        protected static void WriteType_ruuz(ref MessageWriter writer, (uint, uint) value)
        {
            writer.WriteStructureStart();
            writer.WriteUInt32(value.Item1);
            writer.WriteUInt32(value.Item2);
        }
        protected static void WriteType_rddddz(ref MessageWriter writer, (double, double, double, double) value)
        {
            writer.WriteStructureStart();
            writer.WriteDouble(value.Item1);
            writer.WriteDouble(value.Item2);
            writer.WriteDouble(value.Item3);
            writer.WriteDouble(value.Item4);
        }
    }
    class PropertyChanges<TProperties>
    {
        public PropertyChanges(TProperties properties, string[] invalidated, string[] changed)
        	=> (Properties, Invalidated, Changed) = (properties, invalidated, changed);
        public TProperties Properties { get; }
        public string[] Invalidated { get; }
        public string[] Changed { get; }
        public bool HasChanged(string property) => Array.IndexOf(Changed, property) != -1;
        public bool IsInvalidated(string property) => Array.IndexOf(Invalidated, property) != -1;
    }
}
