
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CDP;
using JSON = Newtonsoft.Json.JsonConvert;

namespace CDP.Domains
{
    public class DomainBase
    {
        protected readonly CDP.Chrome chrome;

        public DomainBase(CDP.Chrome chrome)
        {
            this.chrome = chrome;
        }
        protected static T Convert<T>(Dictionary<string, object> data)
        {
            if (data == null)
            {
                return default(T);
            }
            return JSON.DeserializeObject<T>(JSON.SerializeObject(data));
        }
    }

    
    public class Accessibility : DomainBase
    {
        public Accessibility(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> The loadComplete event mirrors the load complete event sent by the browser to assistivetechnology when the web page has finished loading. </summary>
        /// <returns> remove handler </returns>
        public Action OnLoadComplete(Action<OnLoadCompleteParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnLoadCompleteParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Accessibility.loadComplete" : $"Accessibility.loadComplete.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> The nodesUpdated event is sent every time a previously requested node has changed the in tree. </summary>
        /// <returns> remove handler </returns>
        public Action OnNodesUpdated(Action<OnNodesUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnNodesUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Accessibility.nodesUpdated" : $"Accessibility.nodesUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Disables the accessibility domain. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Accessibility.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables the accessibility domain which causes `AXNodeId`s to remain consistent between method calls.This turns on accessibility for the page, which can impact performance until accessibility is disabled. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Accessibility.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Fetches the accessibility node and partial accessibility tree for this DOM node, if it exists. 
        /// </summary>
        public async Task<GetPartialAXTreeReturn> GetPartialAXTree(GetPartialAXTreeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Accessibility.getPartialAXTree", parameters, sessionId);
            return Convert<GetPartialAXTreeReturn>(___r);
        }
        /// <summary> 
        /// Fetches the entire accessibility tree for the root Document 
        /// </summary>
        public async Task<GetFullAXTreeReturn> GetFullAXTree(GetFullAXTreeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Accessibility.getFullAXTree", parameters, sessionId);
            return Convert<GetFullAXTreeReturn>(___r);
        }
        /// <summary> 
        /// Fetches the root node.Requires `enable()` to have been called previously. 
        /// </summary>
        public async Task<GetRootAXNodeReturn> GetRootAXNode(GetRootAXNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Accessibility.getRootAXNode", parameters, sessionId);
            return Convert<GetRootAXNodeReturn>(___r);
        }
        /// <summary> 
        /// Fetches a node and all ancestors up to and including the root.Requires `enable()` to have been called previously. 
        /// </summary>
        public async Task<GetAXNodeAndAncestorsReturn> GetAXNodeAndAncestors(GetAXNodeAndAncestorsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Accessibility.getAXNodeAndAncestors", parameters, sessionId);
            return Convert<GetAXNodeAndAncestorsReturn>(___r);
        }
        /// <summary> 
        /// Fetches a particular accessibility node by AXNodeId.Requires `enable()` to have been called previously. 
        /// </summary>
        public async Task<GetChildAXNodesReturn> GetChildAXNodes(GetChildAXNodesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Accessibility.getChildAXNodes", parameters, sessionId);
            return Convert<GetChildAXNodesReturn>(___r);
        }
        /// <summary> 
        /// Query a DOM node's accessibility subtree for accessible name and role.This command computes the name and role for all nodes in the subtree, including those that areignored for accessibility, and returns those that mactch the specified name and role. If no DOMnode is specified, or the DOM node does not exist, the command returns an error. If neither`accessibleName` or `role` is specified, it returns all the accessibility nodes in the subtree. 
        /// </summary>
        public async Task<QueryAXTreeReturn> QueryAXTree(QueryAXTreeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Accessibility.queryAXTree", parameters, sessionId);
            return Convert<QueryAXTreeReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class AXValueSourceType
        {
            
            /// <summary> What type of source this is. </summary>
            public string type;
            /// <summary> The value of this property source. </summary>
            public Accessibility.AXValueType value;
            /// <summary> The name of the relevant attribute, if any. </summary>
            public string attribute;
            /// <summary> The value of the relevant attribute, if any. </summary>
            public Accessibility.AXValueType attributeValue;
            /// <summary> Whether this source is superseded by a higher priority source. </summary>
            public bool superseded;
            /// <summary> The native markup source for this value, e.g. a <label> element. </summary>
            public string nativeSource;
            /// <summary> The value, such as a node or node list, of the native source. </summary>
            public Accessibility.AXValueType nativeSourceValue;
            /// <summary> Whether the value for this property is invalid. </summary>
            public bool invalid;
            /// <summary> Reason for the value being invalid, if it is. </summary>
            public string invalidReason;
        }
        public class AXRelatedNodeType
        {
            
            /// <summary> The BackendNodeId of the related DOM node. </summary>
            public int backendDOMNodeId;
            /// <summary> The IDRef value provided, if any. </summary>
            public string idref;
            /// <summary> The text alternative of this node in the current context. </summary>
            public string text;
        }
        public class AXPropertyType
        {
            
            /// <summary> The name of this property. </summary>
            public string name;
            /// <summary> The value of this property. </summary>
            public Accessibility.AXValueType value;
        }
        public class AXValueType
        {
            
            /// <summary> The type of this value. </summary>
            public string type;
            /// <summary> The computed value of this property. </summary>
            public object value;
            /// <summary> One or more related nodes, if applicable. </summary>
            public object[] relatedNodes;
            /// <summary> The sources which contributed to the computation of this property. </summary>
            public object[] sources;
        }
        public class AXNodeType
        {
            
            /// <summary> Unique identifier for this node. </summary>
            public string nodeId;
            /// <summary> Whether this node is ignored for accessibility </summary>
            public bool ignored;
            /// <summary> Collection of reasons why this node is hidden. </summary>
            public object[] ignoredReasons;
            /// <summary> This `Node`'s role, whether explicit or implicit. </summary>
            public Accessibility.AXValueType role;
            /// <summary> The accessible name for this `Node`. </summary>
            public Accessibility.AXValueType name;
            /// <summary> The accessible description for this `Node`. </summary>
            public Accessibility.AXValueType description;
            /// <summary> The value for this `Node`. </summary>
            public Accessibility.AXValueType value;
            /// <summary> All other properties </summary>
            public object[] properties;
            /// <summary> ID for this node's parent. </summary>
            public string parentId;
            /// <summary> IDs for each of this node's child nodes. </summary>
            public object[] childIds;
            /// <summary> The backend ID for the associated DOM node, if any. </summary>
            public int backendDOMNodeId;
            /// <summary> The frame ID for the frame associated with this nodes document. </summary>
            public string frameId;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnLoadCompleteParameters
        {
            
            /// <summary> [Require] New document root node. </summary>
            public Accessibility.AXNodeType root;
        }
        public class OnNodesUpdatedParameters
        {
            
            /// <summary> [Require] Updated node data. </summary>
            public object[] nodes;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class GetPartialAXTreeParameters
        {
            
            /// <summary> [Optional] Identifier of the node to get the partial accessibility tree for. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node to get the partial accessibility tree for. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper to get the partial accessibility tree for. </summary>
            public string objectId;
            /// <summary> [Optional] Whether to fetch this nodes ancestors, siblings and children. Defaults to true. </summary>
            public bool fetchRelatives;
        }
        public class GetFullAXTreeParameters
        {
            
            /// <summary> [Optional] The maximum depth at which descendants of the root node should be retrieved.If omitted, the full tree is returned. </summary>
            public int depth;
            /// <summary> [Optional] Deprecated. This parameter has been renamed to `depth`. If depth is not provided, max_depth will be used. </summary>
            public int max_depth;
            /// <summary> [Optional] The frame for whose document the AX tree should be retrieved.If omited, the root frame is used. </summary>
            public string frameId;
        }
        public class GetRootAXNodeParameters
        {
            
            /// <summary> [Optional] The frame in whose document the node resides.If omitted, the root frame is used. </summary>
            public string frameId;
        }
        public class GetAXNodeAndAncestorsParameters
        {
            
            /// <summary> [Optional] Identifier of the node to get. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node to get. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper to get. </summary>
            public string objectId;
        }
        public class GetChildAXNodesParameters
        {
            
            /// <summary> [Require]  </summary>
            public string id;
            /// <summary> [Optional] The frame in whose document the node resides.If omitted, the root frame is used. </summary>
            public string frameId;
        }
        public class QueryAXTreeParameters
        {
            
            /// <summary> [Optional] Identifier of the node for the root to query. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node for the root to query. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper for the root to query. </summary>
            public string objectId;
            /// <summary> [Optional] Find nodes with this computed name. </summary>
            public string accessibleName;
            /// <summary> [Optional] Find nodes with this computed role. </summary>
            public string role;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetPartialAXTreeReturn
        {
            
            /// <summary> The `Accessibility.AXNode` for this DOM node, if it exists, plus its ancestors, siblings andchildren, if requested. </summary>
            public object[] nodes;
        }
        public class GetFullAXTreeReturn
        {
            
            /// <summary>  </summary>
            public object[] nodes;
        }
        public class GetRootAXNodeReturn
        {
            
            /// <summary>  </summary>
            public Accessibility.AXNodeType node;
        }
        public class GetAXNodeAndAncestorsReturn
        {
            
            /// <summary>  </summary>
            public object[] nodes;
        }
        public class GetChildAXNodesReturn
        {
            
            /// <summary>  </summary>
            public object[] nodes;
        }
        public class QueryAXTreeReturn
        {
            
            /// <summary> A list of `Accessibility.AXNode` matching the specified attributes,including nodes that are ignored for accessibility. </summary>
            public object[] nodes;
        }
    }
    
    public class Animation : DomainBase
    {
        public Animation(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Event for when an animation has been cancelled. </summary>
        /// <returns> remove handler </returns>
        public Action OnAnimationCanceled(Action<OnAnimationCanceledParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAnimationCanceledParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Animation.animationCanceled" : $"Animation.animationCanceled.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Event for each animation that has been created. </summary>
        /// <returns> remove handler </returns>
        public Action OnAnimationCreated(Action<OnAnimationCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAnimationCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Animation.animationCreated" : $"Animation.animationCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Event for animation that has been started. </summary>
        /// <returns> remove handler </returns>
        public Action OnAnimationStarted(Action<OnAnimationStartedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAnimationStartedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Animation.animationStarted" : $"Animation.animationStarted.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Disables animation domain notifications. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables animation domain notifications. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns the current time of the an animation. 
        /// </summary>
        public async Task<GetCurrentTimeReturn> GetCurrentTime(GetCurrentTimeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.getCurrentTime", parameters, sessionId);
            return Convert<GetCurrentTimeReturn>(___r);
        }
        /// <summary> 
        /// Gets the playback rate of the document timeline. 
        /// </summary>
        public async Task<GetPlaybackRateReturn> GetPlaybackRate(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.getPlaybackRate", null, sessionId);
            return Convert<GetPlaybackRateReturn>(___r);
        }
        /// <summary> 
        /// Releases a set of animations to no longer be manipulated. 
        /// </summary>
        public async Task ReleaseAnimations(ReleaseAnimationsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.releaseAnimations", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Gets the remote object of the Animation. 
        /// </summary>
        public async Task<ResolveAnimationReturn> ResolveAnimation(ResolveAnimationParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.resolveAnimation", parameters, sessionId);
            return Convert<ResolveAnimationReturn>(___r);
        }
        /// <summary> 
        /// Seek a set of animations to a particular time within each animation. 
        /// </summary>
        public async Task SeekAnimations(SeekAnimationsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.seekAnimations", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets the paused state of a set of animations. 
        /// </summary>
        public async Task SetPaused(SetPausedParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.setPaused", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets the playback rate of the document timeline. 
        /// </summary>
        public async Task SetPlaybackRate(SetPlaybackRateParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.setPlaybackRate", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets the timing of an animation node. 
        /// </summary>
        public async Task SetTiming(SetTimingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Animation.setTiming", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class AnimationType
        {
            
            /// <summary> `Animation`'s id. </summary>
            public string id;
            /// <summary> `Animation`'s name. </summary>
            public string name;
            /// <summary> `Animation`'s internal paused state. </summary>
            public bool pausedState;
            /// <summary> `Animation`'s play state. </summary>
            public string playState;
            /// <summary> `Animation`'s playback rate. </summary>
            public double playbackRate;
            /// <summary> `Animation`'s start time. </summary>
            public double startTime;
            /// <summary> `Animation`'s current time. </summary>
            public double currentTime;
            /// <summary> Animation type of `Animation`. </summary>
            public string type;
            /// <summary> `Animation`'s source animation node. </summary>
            public Animation.AnimationEffectType source;
            /// <summary> A unique ID for `Animation` representing the sources that triggered this CSSanimation/transition. </summary>
            public string cssId;
        }
        public class AnimationEffectType
        {
            
            /// <summary> `AnimationEffect`'s delay. </summary>
            public double delay;
            /// <summary> `AnimationEffect`'s end delay. </summary>
            public double endDelay;
            /// <summary> `AnimationEffect`'s iteration start. </summary>
            public double iterationStart;
            /// <summary> `AnimationEffect`'s iterations. </summary>
            public double iterations;
            /// <summary> `AnimationEffect`'s iteration duration. </summary>
            public double duration;
            /// <summary> `AnimationEffect`'s playback direction. </summary>
            public string direction;
            /// <summary> `AnimationEffect`'s fill mode. </summary>
            public string fill;
            /// <summary> `AnimationEffect`'s target node. </summary>
            public int backendNodeId;
            /// <summary> `AnimationEffect`'s keyframes. </summary>
            public Animation.KeyframesRuleType keyframesRule;
            /// <summary> `AnimationEffect`'s timing function. </summary>
            public string easing;
        }
        public class KeyframesRuleType
        {
            
            /// <summary> CSS keyframed animation's name. </summary>
            public string name;
            /// <summary> List of animation keyframes. </summary>
            public object[] keyframes;
        }
        public class KeyframeStyleType
        {
            
            /// <summary> Keyframe's time offset. </summary>
            public string offset;
            /// <summary> `AnimationEffect`'s timing function. </summary>
            public string easing;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnAnimationCanceledParameters
        {
            
            /// <summary> [Require] Id of the animation that was cancelled. </summary>
            public string id;
        }
        public class OnAnimationCreatedParameters
        {
            
            /// <summary> [Require] Id of the animation that was created. </summary>
            public string id;
        }
        public class OnAnimationStartedParameters
        {
            
            /// <summary> [Require] Animation that was started. </summary>
            public Animation.AnimationType animation;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class GetCurrentTimeParameters
        {
            
            /// <summary> [Require] Id of animation. </summary>
            public string id;
        }
        public class ReleaseAnimationsParameters
        {
            
            /// <summary> [Require] List of animation ids to seek. </summary>
            public object[] animations;
        }
        public class ResolveAnimationParameters
        {
            
            /// <summary> [Require] Animation id. </summary>
            public string animationId;
        }
        public class SeekAnimationsParameters
        {
            
            /// <summary> [Require] List of animation ids to seek. </summary>
            public object[] animations;
            /// <summary> [Require] Set the current time of each animation. </summary>
            public double currentTime;
        }
        public class SetPausedParameters
        {
            
            /// <summary> [Require] Animations to set the pause state of. </summary>
            public object[] animations;
            /// <summary> [Require] Paused state to set to. </summary>
            public bool paused;
        }
        public class SetPlaybackRateParameters
        {
            
            /// <summary> [Require] Playback rate for animations on page </summary>
            public double playbackRate;
        }
        public class SetTimingParameters
        {
            
            /// <summary> [Require] Animation id. </summary>
            public string animationId;
            /// <summary> [Require] Duration of the animation. </summary>
            public double duration;
            /// <summary> [Require] Delay of the animation. </summary>
            public double delay;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetCurrentTimeReturn
        {
            
            /// <summary> Current time of the page. </summary>
            public double currentTime;
        }
        public class GetPlaybackRateReturn
        {
            
            /// <summary> Playback rate for animations on page. </summary>
            public double playbackRate;
        }
        public class ResolveAnimationReturn
        {
            
            /// <summary> Corresponding remote object. </summary>
            public Runtime.RemoteObjectType remoteObject;
        }
    }
    
    public class Audits : DomainBase
    {
        public Audits(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnIssueAdded(Action<OnIssueAddedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnIssueAddedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Audits.issueAdded" : $"Audits.issueAdded.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Returns the response body and size if it were re-encoded with the specified settings. Onlyapplies to images. 
        /// </summary>
        public async Task<GetEncodedResponseReturn> GetEncodedResponse(GetEncodedResponseParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Audits.getEncodedResponse", parameters, sessionId);
            return Convert<GetEncodedResponseReturn>(___r);
        }
        /// <summary> 
        /// Disables issues domain, prevents further issues from being reported to the client. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Audits.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables issues domain, sends the issues collected so far to the client by means of the`issueAdded` event. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Audits.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Runs the contrast check for the target page. Found issues are reportedusing Audits.issueAdded event. 
        /// </summary>
        public async Task CheckContrast(CheckContrastParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Audits.checkContrast", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class AffectedCookieType
        {
            
            /// <summary> The following three properties uniquely identify a cookie </summary>
            public string name;
            /// <summary>  </summary>
            public string path;
            /// <summary>  </summary>
            public string domain;
        }
        public class AffectedRequestType
        {
            
            /// <summary> The unique request id. </summary>
            public string requestId;
            /// <summary>  </summary>
            public string url;
        }
        public class AffectedFrameType
        {
            
            /// <summary>  </summary>
            public string frameId;
        }
        public class SameSiteCookieIssueDetailsType
        {
            
            /// <summary> If AffectedCookie is not set then rawCookieLine contains the rawSet-Cookie header string. This hints at a problem where thecookie line is syntactically or semantically malformed in a waythat no valid cookie could be created. </summary>
            public Audits.AffectedCookieType cookie;
            /// <summary>  </summary>
            public string rawCookieLine;
            /// <summary>  </summary>
            public object[] cookieWarningReasons;
            /// <summary>  </summary>
            public object[] cookieExclusionReasons;
            /// <summary> Optionally identifies the site-for-cookies and the cookie url, whichmay be used by the front-end as additional context. </summary>
            public string operation;
            /// <summary>  </summary>
            public string siteForCookies;
            /// <summary>  </summary>
            public string cookieUrl;
            /// <summary>  </summary>
            public Audits.AffectedRequestType request;
        }
        public class MixedContentIssueDetailsType
        {
            
            /// <summary> The type of resource causing the mixed content issue (css, js, iframe,form,...). Marked as optional because it is mapped to fromblink::mojom::RequestContextType, which will be replacedby network::mojom::RequestDestination </summary>
            public string resourceType;
            /// <summary> The way the mixed content issue is being resolved. </summary>
            public string resolutionStatus;
            /// <summary> The unsafe http url causing the mixed content issue. </summary>
            public string insecureURL;
            /// <summary> The url responsible for the call to an unsafe url. </summary>
            public string mainResourceURL;
            /// <summary> The mixed content request.Does not always exist (e.g. for unsafe form submission urls). </summary>
            public Audits.AffectedRequestType request;
            /// <summary> Optional because not every mixed content issue is necessarily linked to a frame. </summary>
            public Audits.AffectedFrameType frame;
        }
        public class BlockedByResponseIssueDetailsType
        {
            
            /// <summary>  </summary>
            public Audits.AffectedRequestType request;
            /// <summary>  </summary>
            public Audits.AffectedFrameType parentFrame;
            /// <summary>  </summary>
            public Audits.AffectedFrameType blockedFrame;
            /// <summary>  </summary>
            public string reason;
        }
        public class HeavyAdIssueDetailsType
        {
            
            /// <summary> The resolution status, either blocking the content or warning. </summary>
            public string resolution;
            /// <summary> The reason the ad was blocked, total network or cpu or peak cpu. </summary>
            public string reason;
            /// <summary> The frame that was blocked. </summary>
            public Audits.AffectedFrameType frame;
        }
        public class SourceCodeLocationType
        {
            
            /// <summary>  </summary>
            public string scriptId;
            /// <summary>  </summary>
            public string url;
            /// <summary>  </summary>
            public int lineNumber;
            /// <summary>  </summary>
            public int columnNumber;
        }
        public class ContentSecurityPolicyIssueDetailsType
        {
            
            /// <summary> The url not included in allowed sources. </summary>
            public string blockedURL;
            /// <summary> Specific directive that is violated, causing the CSP issue. </summary>
            public string violatedDirective;
            /// <summary>  </summary>
            public bool isReportOnly;
            /// <summary>  </summary>
            public string contentSecurityPolicyViolationType;
            /// <summary>  </summary>
            public Audits.AffectedFrameType frameAncestor;
            /// <summary>  </summary>
            public Audits.SourceCodeLocationType sourceCodeLocation;
            /// <summary>  </summary>
            public int violatingNodeId;
        }
        public class SharedArrayBufferIssueDetailsType
        {
            
            /// <summary>  </summary>
            public Audits.SourceCodeLocationType sourceCodeLocation;
            /// <summary>  </summary>
            public bool isWarning;
            /// <summary>  </summary>
            public string type;
        }
        public class TrustedWebActivityIssueDetailsType
        {
            
            /// <summary> The url that triggers the violation. </summary>
            public string url;
            /// <summary>  </summary>
            public string violationType;
            /// <summary>  </summary>
            public int httpStatusCode;
            /// <summary> The package name of the Trusted Web Activity client app. This field isonly used when violation type is kDigitalAssetLinks. </summary>
            public string packageName;
            /// <summary> The signature of the Trusted Web Activity client app. This field is onlyused when violation type is kDigitalAssetLinks. </summary>
            public string signature;
        }
        public class LowTextContrastIssueDetailsType
        {
            
            /// <summary>  </summary>
            public int violatingNodeId;
            /// <summary>  </summary>
            public string violatingNodeSelector;
            /// <summary>  </summary>
            public double contrastRatio;
            /// <summary>  </summary>
            public double thresholdAA;
            /// <summary>  </summary>
            public double thresholdAAA;
            /// <summary>  </summary>
            public string fontSize;
            /// <summary>  </summary>
            public string fontWeight;
        }
        public class CorsIssueDetailsType
        {
            
            /// <summary>  </summary>
            public Network.CorsErrorStatusType corsErrorStatus;
            /// <summary>  </summary>
            public bool isWarning;
            /// <summary>  </summary>
            public Audits.AffectedRequestType request;
            /// <summary>  </summary>
            public Audits.SourceCodeLocationType location;
            /// <summary>  </summary>
            public string initiatorOrigin;
            /// <summary>  </summary>
            public string resourceIPAddressSpace;
            /// <summary>  </summary>
            public Network.ClientSecurityStateType clientSecurityState;
        }
        public class AttributionReportingIssueDetailsType
        {
            
            /// <summary>  </summary>
            public string violationType;
            /// <summary>  </summary>
            public Audits.AffectedFrameType frame;
            /// <summary>  </summary>
            public Audits.AffectedRequestType request;
            /// <summary>  </summary>
            public int violatingNodeId;
            /// <summary>  </summary>
            public string invalidParameter;
        }
        public class QuirksModeIssueDetailsType
        {
            
            /// <summary> If false, it means the document's mode is "quirks"instead of "limited-quirks". </summary>
            public bool isLimitedQuirksMode;
            /// <summary>  </summary>
            public int documentNodeId;
            /// <summary>  </summary>
            public string url;
            /// <summary>  </summary>
            public string frameId;
            /// <summary>  </summary>
            public string loaderId;
        }
        public class NavigatorUserAgentIssueDetailsType
        {
            
            /// <summary>  </summary>
            public string url;
            /// <summary>  </summary>
            public Audits.SourceCodeLocationType location;
        }
        public class GenericIssueDetailsType
        {
            
            /// <summary> Issues with the same errorType are aggregated in the frontend. </summary>
            public string errorType;
            /// <summary>  </summary>
            public string frameId;
        }
        public class DeprecationIssueDetailsType
        {
            
            /// <summary>  </summary>
            public Audits.AffectedFrameType affectedFrame;
            /// <summary>  </summary>
            public Audits.SourceCodeLocationType sourceCodeLocation;
            /// <summary> The content of the deprecation issue (this won't be translated),e.g. "window.inefficientLegacyStorageMethod will be removed in M97,around January 2022. Please use Web Storage or Indexed Databaseinstead. This standard was abandoned in January, 1970. Seehttps://www.chromestatus.com/feature/5684870116278272 for more details." </summary>
            public string message;
            /// <summary>  </summary>
            public string deprecationType;
        }
        public class FederatedAuthRequestIssueDetailsType
        {
            
            /// <summary>  </summary>
            public string federatedAuthRequestIssueReason;
        }
        public class ClientHintIssueDetailsType
        {
            
            /// <summary>  </summary>
            public Audits.SourceCodeLocationType sourceCodeLocation;
            /// <summary>  </summary>
            public string clientHintIssueReason;
        }
        public class InspectorIssueDetailsType
        {
            
            /// <summary>  </summary>
            public Audits.SameSiteCookieIssueDetailsType sameSiteCookieIssueDetails;
            /// <summary>  </summary>
            public Audits.MixedContentIssueDetailsType mixedContentIssueDetails;
            /// <summary>  </summary>
            public Audits.BlockedByResponseIssueDetailsType blockedByResponseIssueDetails;
            /// <summary>  </summary>
            public Audits.HeavyAdIssueDetailsType heavyAdIssueDetails;
            /// <summary>  </summary>
            public Audits.ContentSecurityPolicyIssueDetailsType contentSecurityPolicyIssueDetails;
            /// <summary>  </summary>
            public Audits.SharedArrayBufferIssueDetailsType sharedArrayBufferIssueDetails;
            /// <summary>  </summary>
            public Audits.TrustedWebActivityIssueDetailsType twaQualityEnforcementDetails;
            /// <summary>  </summary>
            public Audits.LowTextContrastIssueDetailsType lowTextContrastIssueDetails;
            /// <summary>  </summary>
            public Audits.CorsIssueDetailsType corsIssueDetails;
            /// <summary>  </summary>
            public Audits.AttributionReportingIssueDetailsType attributionReportingIssueDetails;
            /// <summary>  </summary>
            public Audits.QuirksModeIssueDetailsType quirksModeIssueDetails;
            /// <summary>  </summary>
            public Audits.NavigatorUserAgentIssueDetailsType navigatorUserAgentIssueDetails;
            /// <summary>  </summary>
            public Audits.GenericIssueDetailsType genericIssueDetails;
            /// <summary>  </summary>
            public Audits.DeprecationIssueDetailsType deprecationIssueDetails;
            /// <summary>  </summary>
            public Audits.ClientHintIssueDetailsType clientHintIssueDetails;
            /// <summary>  </summary>
            public Audits.FederatedAuthRequestIssueDetailsType federatedAuthRequestIssueDetails;
        }
        public class InspectorIssueType
        {
            
            /// <summary>  </summary>
            public string code;
            /// <summary>  </summary>
            public Audits.InspectorIssueDetailsType details;
            /// <summary> A unique id for this issue. May be omitted if no other entity (e.g.exception, CDP message, etc.) is referencing this issue. </summary>
            public string issueId;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnIssueAddedParameters
        {
            
            /// <summary> [Require]  </summary>
            public Audits.InspectorIssueType issue;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class GetEncodedResponseParameters
        {
            
            /// <summary> [Require] Identifier of the network request to get content for. </summary>
            public string requestId;
            /// <summary> [Require] The encoding to use. </summary>
            public string encoding;
            /// <summary> [Optional] The quality of the encoding (0-1). (defaults to 1) </summary>
            public double quality;
            /// <summary> [Optional] Whether to only return the size information (defaults to false). </summary>
            public bool sizeOnly;
        }
        public class CheckContrastParameters
        {
            
            /// <summary> [Optional] Whether to report WCAG AAA level issues. Default is false. </summary>
            public bool reportAAA;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetEncodedResponseReturn
        {
            
            /// <summary> The encoded body as a base64 string. Omitted if sizeOnly is true. (Encoded as a base64 string when passed over JSON) </summary>
            public string body;
            /// <summary> Size before re-encoding. </summary>
            public int originalSize;
            /// <summary> Size after re-encoding. </summary>
            public int encodedSize;
        }
    }
    
    public class BackgroundService : DomainBase
    {
        public BackgroundService(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Called when the recording state for the service has been updated. </summary>
        /// <returns> remove handler </returns>
        public Action OnRecordingStateChanged(Action<OnRecordingStateChangedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnRecordingStateChangedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "BackgroundService.recordingStateChanged" : $"BackgroundService.recordingStateChanged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Called with all existing backgroundServiceEvents when enabled, and all newevents afterwards if enabled and recording. </summary>
        /// <returns> remove handler </returns>
        public Action OnBackgroundServiceEventReceived(Action<OnBackgroundServiceEventReceivedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnBackgroundServiceEventReceivedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "BackgroundService.backgroundServiceEventReceived" : $"BackgroundService.backgroundServiceEventReceived.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Enables event updates for the service. 
        /// </summary>
        public async Task StartObserving(StartObservingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("BackgroundService.startObserving", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables event updates for the service. 
        /// </summary>
        public async Task StopObserving(StopObservingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("BackgroundService.stopObserving", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Set the recording state for the service. 
        /// </summary>
        public async Task SetRecording(SetRecordingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("BackgroundService.setRecording", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears all stored data for the service. 
        /// </summary>
        public async Task ClearEvents(ClearEventsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("BackgroundService.clearEvents", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class EventMetadataType
        {
            
            /// <summary>  </summary>
            public string key;
            /// <summary>  </summary>
            public string value;
        }
        public class BackgroundServiceEventType
        {
            
            /// <summary> Timestamp of the event (in seconds). </summary>
            public double timestamp;
            /// <summary> The origin this event belongs to. </summary>
            public string origin;
            /// <summary> The Service Worker ID that initiated the event. </summary>
            public string serviceWorkerRegistrationId;
            /// <summary> The Background Service this event belongs to. </summary>
            public string service;
            /// <summary> A description of the event. </summary>
            public string eventName;
            /// <summary> An identifier that groups related events together. </summary>
            public string instanceId;
            /// <summary> A list of event-specific information. </summary>
            public object[] eventMetadata;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnRecordingStateChangedParameters
        {
            
            /// <summary> [Require]  </summary>
            public bool isRecording;
            /// <summary> [Require]  </summary>
            public string service;
        }
        public class OnBackgroundServiceEventReceivedParameters
        {
            
            /// <summary> [Require]  </summary>
            public BackgroundService.BackgroundServiceEventType backgroundServiceEvent;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class StartObservingParameters
        {
            
            /// <summary> [Require]  </summary>
            public string service;
        }
        public class StopObservingParameters
        {
            
            /// <summary> [Require]  </summary>
            public string service;
        }
        public class SetRecordingParameters
        {
            
            /// <summary> [Require]  </summary>
            public bool shouldRecord;
            /// <summary> [Require]  </summary>
            public string service;
        }
        public class ClearEventsParameters
        {
            
            /// <summary> [Require]  </summary>
            public string service;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class Browser : DomainBase
    {
        public Browser(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Fired when page is about to start a download. </summary>
        /// <returns> remove handler </returns>
        public Action OnDownloadWillBegin(Action<OnDownloadWillBeginParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDownloadWillBeginParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Browser.downloadWillBegin" : $"Browser.downloadWillBegin.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when download makes progress. Last call has |done| == true. </summary>
        /// <returns> remove handler </returns>
        public Action OnDownloadProgress(Action<OnDownloadProgressParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDownloadProgressParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Browser.downloadProgress" : $"Browser.downloadProgress.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Set permission settings for given origin. 
        /// </summary>
        public async Task SetPermission(SetPermissionParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.setPermission", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Grant specific permissions to the given origin and reject all others. 
        /// </summary>
        public async Task GrantPermissions(GrantPermissionsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.grantPermissions", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Reset all permission management for all origins. 
        /// </summary>
        public async Task ResetPermissions(ResetPermissionsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.resetPermissions", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Set the behavior when downloading a file. 
        /// </summary>
        public async Task SetDownloadBehavior(SetDownloadBehaviorParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.setDownloadBehavior", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Cancel a download if in progress 
        /// </summary>
        public async Task CancelDownload(CancelDownloadParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.cancelDownload", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Close browser gracefully. 
        /// </summary>
        public async Task Close(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.close", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Crashes browser on the main thread. 
        /// </summary>
        public async Task Crash(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.crash", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Crashes GPU process. 
        /// </summary>
        public async Task CrashGpuProcess(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.crashGpuProcess", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns version information. 
        /// </summary>
        public async Task<GetVersionReturn> GetVersion(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.getVersion", null, sessionId);
            return Convert<GetVersionReturn>(___r);
        }
        /// <summary> 
        /// Returns the command line switches for the browser process if, and only if--enable-automation is on the commandline. 
        /// </summary>
        public async Task<GetBrowserCommandLineReturn> GetBrowserCommandLine(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.getBrowserCommandLine", null, sessionId);
            return Convert<GetBrowserCommandLineReturn>(___r);
        }
        /// <summary> 
        /// Get Chrome histograms. 
        /// </summary>
        public async Task<GetHistogramsReturn> GetHistograms(GetHistogramsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.getHistograms", parameters, sessionId);
            return Convert<GetHistogramsReturn>(___r);
        }
        /// <summary> 
        /// Get a Chrome histogram by name. 
        /// </summary>
        public async Task<GetHistogramReturn> GetHistogram(GetHistogramParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.getHistogram", parameters, sessionId);
            return Convert<GetHistogramReturn>(___r);
        }
        /// <summary> 
        /// Get position and size of the browser window. 
        /// </summary>
        public async Task<GetWindowBoundsReturn> GetWindowBounds(GetWindowBoundsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.getWindowBounds", parameters, sessionId);
            return Convert<GetWindowBoundsReturn>(___r);
        }
        /// <summary> 
        /// Get the browser window that contains the devtools target. 
        /// </summary>
        public async Task<GetWindowForTargetReturn> GetWindowForTarget(GetWindowForTargetParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.getWindowForTarget", parameters, sessionId);
            return Convert<GetWindowForTargetReturn>(___r);
        }
        /// <summary> 
        /// Set position and/or size of the browser window. 
        /// </summary>
        public async Task SetWindowBounds(SetWindowBoundsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.setWindowBounds", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Set dock tile details, platform-specific. 
        /// </summary>
        public async Task SetDockTile(SetDockTileParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.setDockTile", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Invoke custom browser commands used by telemetry. 
        /// </summary>
        public async Task ExecuteBrowserCommand(ExecuteBrowserCommandParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Browser.executeBrowserCommand", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class BoundsType
        {
            
            /// <summary> The offset from the left edge of the screen to the window in pixels. </summary>
            public int left;
            /// <summary> The offset from the top edge of the screen to the window in pixels. </summary>
            public int top;
            /// <summary> The window width in pixels. </summary>
            public int width;
            /// <summary> The window height in pixels. </summary>
            public int height;
            /// <summary> The window state. Default to normal. </summary>
            public string windowState;
        }
        public class PermissionDescriptorType
        {
            
            /// <summary> Name of permission.See https://cs.chromium.org/chromium/src/third_party/blink/renderer/modules/permissions/permission_descriptor.idl for valid permission names. </summary>
            public string name;
            /// <summary> For "midi" permission, may also specify sysex control. </summary>
            public bool sysex;
            /// <summary> For "push" permission, may specify userVisibleOnly.Note that userVisibleOnly = true is the only currently supported type. </summary>
            public bool userVisibleOnly;
            /// <summary> For "clipboard" permission, may specify allowWithoutSanitization. </summary>
            public bool allowWithoutSanitization;
            /// <summary> For "camera" permission, may specify panTiltZoom. </summary>
            public bool panTiltZoom;
        }
        public class BucketType
        {
            
            /// <summary> Minimum value (inclusive). </summary>
            public int low;
            /// <summary> Maximum value (exclusive). </summary>
            public int high;
            /// <summary> Number of samples. </summary>
            public int count;
        }
        public class HistogramType
        {
            
            /// <summary> Name. </summary>
            public string name;
            /// <summary> Sum of sample values. </summary>
            public int sum;
            /// <summary> Total number of samples. </summary>
            public int count;
            /// <summary> Buckets. </summary>
            public object[] buckets;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnDownloadWillBeginParameters
        {
            
            /// <summary> [Require] Id of the frame that caused the download to begin. </summary>
            public string frameId;
            /// <summary> [Require] Global unique identifier of the download. </summary>
            public string guid;
            /// <summary> [Require] URL of the resource being downloaded. </summary>
            public string url;
            /// <summary> [Require] Suggested file name of the resource (the actual name of the file saved on disk may differ). </summary>
            public string suggestedFilename;
        }
        public class OnDownloadProgressParameters
        {
            
            /// <summary> [Require] Global unique identifier of the download. </summary>
            public string guid;
            /// <summary> [Require] Total expected bytes to download. </summary>
            public double totalBytes;
            /// <summary> [Require] Total bytes received. </summary>
            public double receivedBytes;
            /// <summary> [Require] Download status. </summary>
            public string state;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class SetPermissionParameters
        {
            
            /// <summary> [Require] Descriptor of permission to override. </summary>
            public Browser.PermissionDescriptorType permission;
            /// <summary> [Require] Setting of the permission. </summary>
            public string setting;
            /// <summary> [Optional] Origin the permission applies to, all origins if not specified. </summary>
            public string origin;
            /// <summary> [Optional] Context to override. When omitted, default browser context is used. </summary>
            public string browserContextId;
        }
        public class GrantPermissionsParameters
        {
            
            /// <summary> [Require]  </summary>
            public object[] permissions;
            /// <summary> [Optional] Origin the permission applies to, all origins if not specified. </summary>
            public string origin;
            /// <summary> [Optional] BrowserContext to override permissions. When omitted, default browser context is used. </summary>
            public string browserContextId;
        }
        public class ResetPermissionsParameters
        {
            
            /// <summary> [Optional] BrowserContext to reset permissions. When omitted, default browser context is used. </summary>
            public string browserContextId;
        }
        public class SetDownloadBehaviorParameters
        {
            
            /// <summary> [Require] Whether to allow all or deny all download requests, or use default Chrome behavior ifavailable (otherwise deny). |allowAndName| allows download and names files according totheir dowmload guids. </summary>
            public string behavior;
            /// <summary> [Optional] BrowserContext to set download behavior. When omitted, default browser context is used. </summary>
            public string browserContextId;
            /// <summary> [Optional] The default path to save downloaded files to. This is required if behavior is set to 'allow'or 'allowAndName'. </summary>
            public string downloadPath;
            /// <summary> [Optional] Whether to emit download events (defaults to false). </summary>
            public bool eventsEnabled;
        }
        public class CancelDownloadParameters
        {
            
            /// <summary> [Require] Global unique identifier of the download. </summary>
            public string guid;
            /// <summary> [Optional] BrowserContext to perform the action in. When omitted, default browser context is used. </summary>
            public string browserContextId;
        }
        public class GetHistogramsParameters
        {
            
            /// <summary> [Optional] Requested substring in name. Only histograms which have query as asubstring in their name are extracted. An empty or absent query returnsall histograms. </summary>
            public string query;
            /// <summary> [Optional] If true, retrieve delta since last call. </summary>
            public bool delta;
        }
        public class GetHistogramParameters
        {
            
            /// <summary> [Require] Requested histogram name. </summary>
            public string name;
            /// <summary> [Optional] If true, retrieve delta since last call. </summary>
            public bool delta;
        }
        public class GetWindowBoundsParameters
        {
            
            /// <summary> [Require] Browser window id. </summary>
            public int windowId;
        }
        public class GetWindowForTargetParameters
        {
            
            /// <summary> [Optional] Devtools agent host id. If called as a part of the session, associated targetId is used. </summary>
            public string targetId;
        }
        public class SetWindowBoundsParameters
        {
            
            /// <summary> [Require] Browser window id. </summary>
            public int windowId;
            /// <summary> [Require] New window bounds. The 'minimized', 'maximized' and 'fullscreen' states cannot be combinedwith 'left', 'top', 'width' or 'height'. Leaves unspecified fields unchanged. </summary>
            public Browser.BoundsType bounds;
        }
        public class SetDockTileParameters
        {
            
            /// <summary> [Optional]  </summary>
            public string badgeLabel;
            /// <summary> [Optional] Png encoded image. (Encoded as a base64 string when passed over JSON) </summary>
            public string image;
        }
        public class ExecuteBrowserCommandParameters
        {
            
            /// <summary> [Require]  </summary>
            public string commandId;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetVersionReturn
        {
            
            /// <summary> Protocol version. </summary>
            public string protocolVersion;
            /// <summary> Product name. </summary>
            public string product;
            /// <summary> Product revision. </summary>
            public string revision;
            /// <summary> User-Agent. </summary>
            public string userAgent;
            /// <summary> V8 version. </summary>
            public string jsVersion;
        }
        public class GetBrowserCommandLineReturn
        {
            
            /// <summary> Commandline parameters </summary>
            public object[] arguments;
        }
        public class GetHistogramsReturn
        {
            
            /// <summary> Histograms. </summary>
            public object[] histograms;
        }
        public class GetHistogramReturn
        {
            
            /// <summary> Histogram. </summary>
            public Browser.HistogramType histogram;
        }
        public class GetWindowBoundsReturn
        {
            
            /// <summary> Bounds information of the window. When window state is 'minimized', the restored windowposition and size are returned. </summary>
            public Browser.BoundsType bounds;
        }
        public class GetWindowForTargetReturn
        {
            
            /// <summary> Browser window id. </summary>
            public int windowId;
            /// <summary> Bounds information of the window. When window state is 'minimized', the restored windowposition and size are returned. </summary>
            public Browser.BoundsType bounds;
        }
    }
    
    public class CSS : DomainBase
    {
        public CSS(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Fires whenever a web font is updated.  A non-empty font parameter indicates a successfully loadedweb font </summary>
        /// <returns> remove handler </returns>
        public Action OnFontsUpdated(Action<OnFontsUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFontsUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "CSS.fontsUpdated" : $"CSS.fontsUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fires whenever a MediaQuery result changes (for example, after a browser window has beenresized.) The current implementation considers only viewport-dependent media features. </summary>
        /// <returns> remove handler </returns>
        public Action OnMediaQueryResultChanged(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "CSS.mediaQueryResultChanged" : $"CSS.mediaQueryResultChanged.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        
        /// <summary> Fired whenever an active document stylesheet is added. </summary>
        /// <returns> remove handler </returns>
        public Action OnStyleSheetAdded(Action<OnStyleSheetAddedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnStyleSheetAddedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "CSS.styleSheetAdded" : $"CSS.styleSheetAdded.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired whenever a stylesheet is changed as a result of the client operation. </summary>
        /// <returns> remove handler </returns>
        public Action OnStyleSheetChanged(Action<OnStyleSheetChangedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnStyleSheetChangedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "CSS.styleSheetChanged" : $"CSS.styleSheetChanged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired whenever an active document stylesheet is removed. </summary>
        /// <returns> remove handler </returns>
        public Action OnStyleSheetRemoved(Action<OnStyleSheetRemovedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnStyleSheetRemovedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "CSS.styleSheetRemoved" : $"CSS.styleSheetRemoved.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Inserts a new rule with the given `ruleText` in a stylesheet with given `styleSheetId`, at theposition specified by `location`. 
        /// </summary>
        public async Task<AddRuleReturn> AddRule(AddRuleParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.addRule", parameters, sessionId);
            return Convert<AddRuleReturn>(___r);
        }
        /// <summary> 
        /// Returns all class names from specified stylesheet. 
        /// </summary>
        public async Task<CollectClassNamesReturn> CollectClassNames(CollectClassNamesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.collectClassNames", parameters, sessionId);
            return Convert<CollectClassNamesReturn>(___r);
        }
        /// <summary> 
        /// Creates a new special "via-inspector" stylesheet in the frame with given `frameId`. 
        /// </summary>
        public async Task<CreateStyleSheetReturn> CreateStyleSheet(CreateStyleSheetParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.createStyleSheet", parameters, sessionId);
            return Convert<CreateStyleSheetReturn>(___r);
        }
        /// <summary> 
        /// Disables the CSS agent for the given page. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables the CSS agent for the given page. Clients should not assume that the CSS agent has beenenabled until the result of this command is received. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Ensures that the given node will have specified pseudo-classes whenever its style is computed bythe browser. 
        /// </summary>
        public async Task ForcePseudoState(ForcePseudoStateParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.forcePseudoState", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetBackgroundColorsReturn> GetBackgroundColors(GetBackgroundColorsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.getBackgroundColors", parameters, sessionId);
            return Convert<GetBackgroundColorsReturn>(___r);
        }
        /// <summary> 
        /// Returns the computed style for a DOM node identified by `nodeId`. 
        /// </summary>
        public async Task<GetComputedStyleForNodeReturn> GetComputedStyleForNode(GetComputedStyleForNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.getComputedStyleForNode", parameters, sessionId);
            return Convert<GetComputedStyleForNodeReturn>(___r);
        }
        /// <summary> 
        /// Returns the styles defined inline (explicitly in the "style" attribute and implicitly, using DOMattributes) for a DOM node identified by `nodeId`. 
        /// </summary>
        public async Task<GetInlineStylesForNodeReturn> GetInlineStylesForNode(GetInlineStylesForNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.getInlineStylesForNode", parameters, sessionId);
            return Convert<GetInlineStylesForNodeReturn>(___r);
        }
        /// <summary> 
        /// Returns requested styles for a DOM node identified by `nodeId`. 
        /// </summary>
        public async Task<GetMatchedStylesForNodeReturn> GetMatchedStylesForNode(GetMatchedStylesForNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.getMatchedStylesForNode", parameters, sessionId);
            return Convert<GetMatchedStylesForNodeReturn>(___r);
        }
        /// <summary> 
        /// Returns all media queries parsed by the rendering engine. 
        /// </summary>
        public async Task<GetMediaQueriesReturn> GetMediaQueries(string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.getMediaQueries", null, sessionId);
            return Convert<GetMediaQueriesReturn>(___r);
        }
        /// <summary> 
        /// Requests information about platform fonts which we used to render child TextNodes in the givennode. 
        /// </summary>
        public async Task<GetPlatformFontsForNodeReturn> GetPlatformFontsForNode(GetPlatformFontsForNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.getPlatformFontsForNode", parameters, sessionId);
            return Convert<GetPlatformFontsForNodeReturn>(___r);
        }
        /// <summary> 
        /// Returns the current textual content for a stylesheet. 
        /// </summary>
        public async Task<GetStyleSheetTextReturn> GetStyleSheetText(GetStyleSheetTextParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.getStyleSheetText", parameters, sessionId);
            return Convert<GetStyleSheetTextReturn>(___r);
        }
        /// <summary> 
        /// Starts tracking the given computed styles for updates. The specified array of propertiesreplaces the one previously specified. Pass empty array to disable tracking.Use takeComputedStyleUpdates to retrieve the list of nodes that had properties modified.The changes to computed style properties are only tracked for nodes pushed to the front-endby the DOM agent. If no changes to the tracked properties occur after the node has been pushedto the front-end, no updates will be issued for the node. 
        /// </summary>
        public async Task TrackComputedStyleUpdates(TrackComputedStyleUpdatesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.trackComputedStyleUpdates", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Polls the next batch of computed style updates. 
        /// </summary>
        public async Task<TakeComputedStyleUpdatesReturn> TakeComputedStyleUpdates(string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.takeComputedStyleUpdates", null, sessionId);
            return Convert<TakeComputedStyleUpdatesReturn>(___r);
        }
        /// <summary> 
        /// Find a rule with the given active property for the given node and set the new value for thisproperty 
        /// </summary>
        public async Task SetEffectivePropertyValueForNode(SetEffectivePropertyValueForNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.setEffectivePropertyValueForNode", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Modifies the keyframe rule key text. 
        /// </summary>
        public async Task<SetKeyframeKeyReturn> SetKeyframeKey(SetKeyframeKeyParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.setKeyframeKey", parameters, sessionId);
            return Convert<SetKeyframeKeyReturn>(___r);
        }
        /// <summary> 
        /// Modifies the rule selector. 
        /// </summary>
        public async Task<SetMediaTextReturn> SetMediaText(SetMediaTextParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.setMediaText", parameters, sessionId);
            return Convert<SetMediaTextReturn>(___r);
        }
        /// <summary> 
        /// Modifies the expression of a container query. 
        /// </summary>
        public async Task<SetContainerQueryTextReturn> SetContainerQueryText(SetContainerQueryTextParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.setContainerQueryText", parameters, sessionId);
            return Convert<SetContainerQueryTextReturn>(___r);
        }
        /// <summary> 
        /// Modifies the rule selector. 
        /// </summary>
        public async Task<SetRuleSelectorReturn> SetRuleSelector(SetRuleSelectorParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.setRuleSelector", parameters, sessionId);
            return Convert<SetRuleSelectorReturn>(___r);
        }
        /// <summary> 
        /// Sets the new stylesheet text. 
        /// </summary>
        public async Task<SetStyleSheetTextReturn> SetStyleSheetText(SetStyleSheetTextParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.setStyleSheetText", parameters, sessionId);
            return Convert<SetStyleSheetTextReturn>(___r);
        }
        /// <summary> 
        /// Applies specified style edits one after another in the given order. 
        /// </summary>
        public async Task<SetStyleTextsReturn> SetStyleTexts(SetStyleTextsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.setStyleTexts", parameters, sessionId);
            return Convert<SetStyleTextsReturn>(___r);
        }
        /// <summary> 
        /// Enables the selector recording. 
        /// </summary>
        public async Task StartRuleUsageTracking(string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.startRuleUsageTracking", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Stop tracking rule usage and return the list of rules that were used since last call to`takeCoverageDelta` (or since start of coverage instrumentation) 
        /// </summary>
        public async Task<StopRuleUsageTrackingReturn> StopRuleUsageTracking(string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.stopRuleUsageTracking", null, sessionId);
            return Convert<StopRuleUsageTrackingReturn>(___r);
        }
        /// <summary> 
        /// Obtain list of rules that became used since last call to this method (or since start of coverageinstrumentation) 
        /// </summary>
        public async Task<TakeCoverageDeltaReturn> TakeCoverageDelta(string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.takeCoverageDelta", null, sessionId);
            return Convert<TakeCoverageDeltaReturn>(___r);
        }
        /// <summary> 
        /// Enables/disables rendering of local CSS fonts (enabled by default). 
        /// </summary>
        public async Task SetLocalFontsEnabled(SetLocalFontsEnabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CSS.setLocalFontsEnabled", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class PseudoElementMatchesType
        {
            
            /// <summary> Pseudo element type. </summary>
            public string pseudoType;
            /// <summary> Matches of CSS rules applicable to the pseudo style. </summary>
            public object[] matches;
        }
        public class InheritedStyleEntryType
        {
            
            /// <summary> The ancestor node's inline style, if any, in the style inheritance chain. </summary>
            public CSS.CSSStyleType inlineStyle;
            /// <summary> Matches of CSS rules matching the ancestor node in the style inheritance chain. </summary>
            public object[] matchedCSSRules;
        }
        public class RuleMatchType
        {
            
            /// <summary> CSS rule in the match. </summary>
            public CSS.CSSRuleType rule;
            /// <summary> Matching selector indices in the rule's selectorList selectors (0-based). </summary>
            public object[] matchingSelectors;
        }
        public class ValueType
        {
            
            /// <summary> Value text. </summary>
            public string text;
            /// <summary> Value range in the underlying resource (if available). </summary>
            public CSS.SourceRangeType range;
        }
        public class SelectorListType
        {
            
            /// <summary> Selectors in the list. </summary>
            public object[] selectors;
            /// <summary> Rule selector text. </summary>
            public string text;
        }
        public class CSSStyleSheetHeaderType
        {
            
            /// <summary> The stylesheet identifier. </summary>
            public string styleSheetId;
            /// <summary> Owner frame identifier. </summary>
            public string frameId;
            /// <summary> Stylesheet resource URL. Empty if this is a constructed stylesheet created usingnew CSSStyleSheet() (but non-empty if this is a constructed sylesheet importedas a CSS module script). </summary>
            public string sourceURL;
            /// <summary> URL of source map associated with the stylesheet (if any). </summary>
            public string sourceMapURL;
            /// <summary> Stylesheet origin. </summary>
            public string origin;
            /// <summary> Stylesheet title. </summary>
            public string title;
            /// <summary> The backend id for the owner node of the stylesheet. </summary>
            public int ownerNode;
            /// <summary> Denotes whether the stylesheet is disabled. </summary>
            public bool disabled;
            /// <summary> Whether the sourceURL field value comes from the sourceURL comment. </summary>
            public bool hasSourceURL;
            /// <summary> Whether this stylesheet is created for STYLE tag by parser. This flag is not set fordocument.written STYLE tags. </summary>
            public bool isInline;
            /// <summary> Whether this stylesheet is mutable. Inline stylesheets become mutableafter they have been modified via CSSOM API.<link> element's stylesheets become mutable only if DevTools modifies them.Constructed stylesheets (new CSSStyleSheet()) are mutable immediately after creation. </summary>
            public bool isMutable;
            /// <summary> True if this stylesheet is created through new CSSStyleSheet() or imported as aCSS module script. </summary>
            public bool isConstructed;
            /// <summary> Line offset of the stylesheet within the resource (zero based). </summary>
            public double startLine;
            /// <summary> Column offset of the stylesheet within the resource (zero based). </summary>
            public double startColumn;
            /// <summary> Size of the content (in characters). </summary>
            public double length;
            /// <summary> Line offset of the end of the stylesheet within the resource (zero based). </summary>
            public double endLine;
            /// <summary> Column offset of the end of the stylesheet within the resource (zero based). </summary>
            public double endColumn;
        }
        public class CSSRuleType
        {
            
            /// <summary> The css style sheet identifier (absent for user agent stylesheet and user-specifiedstylesheet rules) this rule came from. </summary>
            public string styleSheetId;
            /// <summary> Rule selector data. </summary>
            public CSS.SelectorListType selectorList;
            /// <summary> Parent stylesheet's origin. </summary>
            public string origin;
            /// <summary> Associated style declaration. </summary>
            public CSS.CSSStyleType style;
            /// <summary> Media list array (for rules involving media queries). The array enumerates media queriesstarting with the innermost one, going outwards. </summary>
            public object[] media;
            /// <summary> Container query list array (for rules involving container queries).The array enumerates container queries starting with the innermost one, going outwards. </summary>
            public object[] containerQueries;
            /// <summary> @supports CSS at-rule array.The array enumerates @supports at-rules starting with the innermost one, going outwards. </summary>
            public object[] supports;
        }
        public class RuleUsageType
        {
            
            /// <summary> The css style sheet identifier (absent for user agent stylesheet and user-specifiedstylesheet rules) this rule came from. </summary>
            public string styleSheetId;
            /// <summary> Offset of the start of the rule (including selector) from the beginning of the stylesheet. </summary>
            public double startOffset;
            /// <summary> Offset of the end of the rule body from the beginning of the stylesheet. </summary>
            public double endOffset;
            /// <summary> Indicates whether the rule was actually used by some element in the page. </summary>
            public bool used;
        }
        public class SourceRangeType
        {
            
            /// <summary> Start line of range. </summary>
            public int startLine;
            /// <summary> Start column of range (inclusive). </summary>
            public int startColumn;
            /// <summary> End line of range </summary>
            public int endLine;
            /// <summary> End column of range (exclusive). </summary>
            public int endColumn;
        }
        public class ShorthandEntryType
        {
            
            /// <summary> Shorthand name. </summary>
            public string name;
            /// <summary> Shorthand value. </summary>
            public string value;
            /// <summary> Whether the property has "!important" annotation (implies `false` if absent). </summary>
            public bool important;
        }
        public class CSSComputedStylePropertyType
        {
            
            /// <summary> Computed style property name. </summary>
            public string name;
            /// <summary> Computed style property value. </summary>
            public string value;
        }
        public class CSSStyleType
        {
            
            /// <summary> The css style sheet identifier (absent for user agent stylesheet and user-specifiedstylesheet rules) this rule came from. </summary>
            public string styleSheetId;
            /// <summary> CSS properties in the style. </summary>
            public object[] cssProperties;
            /// <summary> Computed values for all shorthands found in the style. </summary>
            public object[] shorthandEntries;
            /// <summary> Style declaration text (if available). </summary>
            public string cssText;
            /// <summary> Style declaration range in the enclosing stylesheet (if available). </summary>
            public CSS.SourceRangeType range;
        }
        public class CSSPropertyType
        {
            
            /// <summary> The property name. </summary>
            public string name;
            /// <summary> The property value. </summary>
            public string value;
            /// <summary> Whether the property has "!important" annotation (implies `false` if absent). </summary>
            public bool important;
            /// <summary> Whether the property is implicit (implies `false` if absent). </summary>
            public bool @implicit;
            /// <summary> The full property text as specified in the style. </summary>
            public string text;
            /// <summary> Whether the property is understood by the browser (implies `true` if absent). </summary>
            public bool parsedOk;
            /// <summary> Whether the property is disabled by the user (present for source-based properties only). </summary>
            public bool disabled;
            /// <summary> The entire property range in the enclosing style declaration (if available). </summary>
            public CSS.SourceRangeType range;
        }
        public class CSSMediaType
        {
            
            /// <summary> Media query text. </summary>
            public string text;
            /// <summary> Source of the media query: "mediaRule" if specified by a @media rule, "importRule" ifspecified by an @import rule, "linkedSheet" if specified by a "media" attribute in a linkedstylesheet's LINK tag, "inlineSheet" if specified by a "media" attribute in an inlinestylesheet's STYLE tag. </summary>
            public string source;
            /// <summary> URL of the document containing the media query description. </summary>
            public string sourceURL;
            /// <summary> The associated rule (@media or @import) header range in the enclosing stylesheet (ifavailable). </summary>
            public CSS.SourceRangeType range;
            /// <summary> Identifier of the stylesheet containing this object (if exists). </summary>
            public string styleSheetId;
            /// <summary> Array of media queries. </summary>
            public object[] mediaList;
        }
        public class MediaQueryType
        {
            
            /// <summary> Array of media query expressions. </summary>
            public object[] expressions;
            /// <summary> Whether the media query condition is satisfied. </summary>
            public bool active;
        }
        public class MediaQueryExpressionType
        {
            
            /// <summary> Media query expression value. </summary>
            public double value;
            /// <summary> Media query expression units. </summary>
            public string unit;
            /// <summary> Media query expression feature. </summary>
            public string feature;
            /// <summary> The associated range of the value text in the enclosing stylesheet (if available). </summary>
            public CSS.SourceRangeType valueRange;
            /// <summary> Computed length of media query expression (if applicable). </summary>
            public double computedLength;
        }
        public class CSSContainerQueryType
        {
            
            /// <summary> Container query text. </summary>
            public string text;
            /// <summary> The associated rule header range in the enclosing stylesheet (ifavailable). </summary>
            public CSS.SourceRangeType range;
            /// <summary> Identifier of the stylesheet containing this object (if exists). </summary>
            public string styleSheetId;
            /// <summary> Optional name for the container. </summary>
            public string name;
        }
        public class CSSSupportsType
        {
            
            /// <summary> Supports rule text. </summary>
            public string text;
            /// <summary> The associated rule header range in the enclosing stylesheet (ifavailable). </summary>
            public CSS.SourceRangeType range;
            /// <summary> Identifier of the stylesheet containing this object (if exists). </summary>
            public string styleSheetId;
        }
        public class PlatformFontUsageType
        {
            
            /// <summary> Font's family name reported by platform. </summary>
            public string familyName;
            /// <summary> Indicates if the font was downloaded or resolved locally. </summary>
            public bool isCustomFont;
            /// <summary> Amount of glyphs that were rendered with this font. </summary>
            public double glyphCount;
        }
        public class FontVariationAxisType
        {
            
            /// <summary> The font-variation-setting tag (a.k.a. "axis tag"). </summary>
            public string tag;
            /// <summary> Human-readable variation name in the default language (normally, "en"). </summary>
            public string name;
            /// <summary> The minimum value (inclusive) the font supports for this tag. </summary>
            public double minValue;
            /// <summary> The maximum value (inclusive) the font supports for this tag. </summary>
            public double maxValue;
            /// <summary> The default value. </summary>
            public double defaultValue;
        }
        public class FontFaceType
        {
            
            /// <summary> The font-family. </summary>
            public string fontFamily;
            /// <summary> The font-style. </summary>
            public string fontStyle;
            /// <summary> The font-variant. </summary>
            public string fontVariant;
            /// <summary> The font-weight. </summary>
            public string fontWeight;
            /// <summary> The font-stretch. </summary>
            public string fontStretch;
            /// <summary> The unicode-range. </summary>
            public string unicodeRange;
            /// <summary> The src. </summary>
            public string src;
            /// <summary> The resolved platform font family </summary>
            public string platformFontFamily;
            /// <summary> Available variation settings (a.k.a. "axes"). </summary>
            public object[] fontVariationAxes;
        }
        public class CSSKeyframesRuleType
        {
            
            /// <summary> Animation name. </summary>
            public CSS.ValueType animationName;
            /// <summary> List of keyframes. </summary>
            public object[] keyframes;
        }
        public class CSSKeyframeRuleType
        {
            
            /// <summary> The css style sheet identifier (absent for user agent stylesheet and user-specifiedstylesheet rules) this rule came from. </summary>
            public string styleSheetId;
            /// <summary> Parent stylesheet's origin. </summary>
            public string origin;
            /// <summary> Associated key text. </summary>
            public CSS.ValueType keyText;
            /// <summary> Associated style declaration. </summary>
            public CSS.CSSStyleType style;
        }
        public class StyleDeclarationEditType
        {
            
            /// <summary> The css style sheet identifier. </summary>
            public string styleSheetId;
            /// <summary> The range of the style text in the enclosing stylesheet. </summary>
            public CSS.SourceRangeType range;
            /// <summary> New style text. </summary>
            public string text;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnFontsUpdatedParameters
        {
            
            /// <summary> [Optional] The web font that has loaded. </summary>
            public CSS.FontFaceType font;
        }
        public class OnStyleSheetAddedParameters
        {
            
            /// <summary> [Require] Added stylesheet metainfo. </summary>
            public CSS.CSSStyleSheetHeaderType header;
        }
        public class OnStyleSheetChangedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string styleSheetId;
        }
        public class OnStyleSheetRemovedParameters
        {
            
            /// <summary> [Require] Identifier of the removed stylesheet. </summary>
            public string styleSheetId;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class AddRuleParameters
        {
            
            /// <summary> [Require] The css style sheet identifier where a new rule should be inserted. </summary>
            public string styleSheetId;
            /// <summary> [Require] The text of a new rule. </summary>
            public string ruleText;
            /// <summary> [Require] Text position of a new rule in the target style sheet. </summary>
            public CSS.SourceRangeType location;
        }
        public class CollectClassNamesParameters
        {
            
            /// <summary> [Require]  </summary>
            public string styleSheetId;
        }
        public class CreateStyleSheetParameters
        {
            
            /// <summary> [Require] Identifier of the frame where "via-inspector" stylesheet should be created. </summary>
            public string frameId;
        }
        public class ForcePseudoStateParameters
        {
            
            /// <summary> [Require] The element id for which to force the pseudo state. </summary>
            public int nodeId;
            /// <summary> [Require] Element pseudo classes to force when computing the element's style. </summary>
            public object[] forcedPseudoClasses;
        }
        public class GetBackgroundColorsParameters
        {
            
            /// <summary> [Require] Id of the node to get background colors for. </summary>
            public int nodeId;
        }
        public class GetComputedStyleForNodeParameters
        {
            
            /// <summary> [Require]  </summary>
            public int nodeId;
        }
        public class GetInlineStylesForNodeParameters
        {
            
            /// <summary> [Require]  </summary>
            public int nodeId;
        }
        public class GetMatchedStylesForNodeParameters
        {
            
            /// <summary> [Require]  </summary>
            public int nodeId;
        }
        public class GetPlatformFontsForNodeParameters
        {
            
            /// <summary> [Require]  </summary>
            public int nodeId;
        }
        public class GetStyleSheetTextParameters
        {
            
            /// <summary> [Require]  </summary>
            public string styleSheetId;
        }
        public class TrackComputedStyleUpdatesParameters
        {
            
            /// <summary> [Require]  </summary>
            public object[] propertiesToTrack;
        }
        public class SetEffectivePropertyValueForNodeParameters
        {
            
            /// <summary> [Require] The element id for which to set property. </summary>
            public int nodeId;
            /// <summary> [Require]  </summary>
            public string propertyName;
            /// <summary> [Require]  </summary>
            public string value;
        }
        public class SetKeyframeKeyParameters
        {
            
            /// <summary> [Require]  </summary>
            public string styleSheetId;
            /// <summary> [Require]  </summary>
            public CSS.SourceRangeType range;
            /// <summary> [Require]  </summary>
            public string keyText;
        }
        public class SetMediaTextParameters
        {
            
            /// <summary> [Require]  </summary>
            public string styleSheetId;
            /// <summary> [Require]  </summary>
            public CSS.SourceRangeType range;
            /// <summary> [Require]  </summary>
            public string text;
        }
        public class SetContainerQueryTextParameters
        {
            
            /// <summary> [Require]  </summary>
            public string styleSheetId;
            /// <summary> [Require]  </summary>
            public CSS.SourceRangeType range;
            /// <summary> [Require]  </summary>
            public string text;
        }
        public class SetRuleSelectorParameters
        {
            
            /// <summary> [Require]  </summary>
            public string styleSheetId;
            /// <summary> [Require]  </summary>
            public CSS.SourceRangeType range;
            /// <summary> [Require]  </summary>
            public string selector;
        }
        public class SetStyleSheetTextParameters
        {
            
            /// <summary> [Require]  </summary>
            public string styleSheetId;
            /// <summary> [Require]  </summary>
            public string text;
        }
        public class SetStyleTextsParameters
        {
            
            /// <summary> [Require]  </summary>
            public object[] edits;
        }
        public class SetLocalFontsEnabledParameters
        {
            
            /// <summary> [Require] Whether rendering of local fonts is enabled. </summary>
            public bool enabled;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class AddRuleReturn
        {
            
            /// <summary> The newly created rule. </summary>
            public CSS.CSSRuleType rule;
        }
        public class CollectClassNamesReturn
        {
            
            /// <summary> Class name list. </summary>
            public object[] classNames;
        }
        public class CreateStyleSheetReturn
        {
            
            /// <summary> Identifier of the created "via-inspector" stylesheet. </summary>
            public string styleSheetId;
        }
        public class GetBackgroundColorsReturn
        {
            
            /// <summary> The range of background colors behind this element, if it contains any visible text. If novisible text is present, this will be undefined. In the case of a flat background color,this will consist of simply that color. In the case of a gradient, this will consist of eachof the color stops. For anything more complicated, this will be an empty array. Images willbe ignored (as if the image had failed to load). </summary>
            public object[] backgroundColors;
            /// <summary> The computed font size for this node, as a CSS computed value string (e.g. '12px'). </summary>
            public string computedFontSize;
            /// <summary> The computed font weight for this node, as a CSS computed value string (e.g. 'normal' or'100'). </summary>
            public string computedFontWeight;
        }
        public class GetComputedStyleForNodeReturn
        {
            
            /// <summary> Computed style for the specified DOM node. </summary>
            public object[] computedStyle;
        }
        public class GetInlineStylesForNodeReturn
        {
            
            /// <summary> Inline style for the specified DOM node. </summary>
            public CSS.CSSStyleType inlineStyle;
            /// <summary> Attribute-defined element style (e.g. resulting from "width=20 height=100%"). </summary>
            public CSS.CSSStyleType attributesStyle;
        }
        public class GetMatchedStylesForNodeReturn
        {
            
            /// <summary> Inline style for the specified DOM node. </summary>
            public CSS.CSSStyleType inlineStyle;
            /// <summary> Attribute-defined element style (e.g. resulting from "width=20 height=100%"). </summary>
            public CSS.CSSStyleType attributesStyle;
            /// <summary> CSS rules matching this node, from all applicable stylesheets. </summary>
            public object[] matchedCSSRules;
            /// <summary> Pseudo style matches for this node. </summary>
            public object[] pseudoElements;
            /// <summary> A chain of inherited styles (from the immediate node parent up to the DOM tree root). </summary>
            public object[] inherited;
            /// <summary> A list of CSS keyframed animations matching this node. </summary>
            public object[] cssKeyframesRules;
        }
        public class GetMediaQueriesReturn
        {
            
            /// <summary>  </summary>
            public object[] medias;
        }
        public class GetPlatformFontsForNodeReturn
        {
            
            /// <summary> Usage statistics for every employed platform font. </summary>
            public object[] fonts;
        }
        public class GetStyleSheetTextReturn
        {
            
            /// <summary> The stylesheet text. </summary>
            public string text;
        }
        public class TakeComputedStyleUpdatesReturn
        {
            
            /// <summary> The list of node Ids that have their tracked computed styles updated </summary>
            public object[] nodeIds;
        }
        public class SetKeyframeKeyReturn
        {
            
            /// <summary> The resulting key text after modification. </summary>
            public CSS.ValueType keyText;
        }
        public class SetMediaTextReturn
        {
            
            /// <summary> The resulting CSS media rule after modification. </summary>
            public CSS.CSSMediaType media;
        }
        public class SetContainerQueryTextReturn
        {
            
            /// <summary> The resulting CSS container query rule after modification. </summary>
            public CSS.CSSContainerQueryType containerQuery;
        }
        public class SetRuleSelectorReturn
        {
            
            /// <summary> The resulting selector list after modification. </summary>
            public CSS.SelectorListType selectorList;
        }
        public class SetStyleSheetTextReturn
        {
            
            /// <summary> URL of source map associated with script (if any). </summary>
            public string sourceMapURL;
        }
        public class SetStyleTextsReturn
        {
            
            /// <summary> The resulting styles after modification. </summary>
            public object[] styles;
        }
        public class StopRuleUsageTrackingReturn
        {
            
            /// <summary>  </summary>
            public object[] ruleUsage;
        }
        public class TakeCoverageDeltaReturn
        {
            
            /// <summary>  </summary>
            public object[] coverage;
            /// <summary> Monotonically increasing time, in seconds. </summary>
            public double timestamp;
        }
    }
    
    public class CacheStorage : DomainBase
    {
        public CacheStorage(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Deletes a cache. 
        /// </summary>
        public async Task DeleteCache(DeleteCacheParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CacheStorage.deleteCache", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Deletes a cache entry. 
        /// </summary>
        public async Task DeleteEntry(DeleteEntryParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CacheStorage.deleteEntry", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests cache names. 
        /// </summary>
        public async Task<RequestCacheNamesReturn> RequestCacheNames(RequestCacheNamesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CacheStorage.requestCacheNames", parameters, sessionId);
            return Convert<RequestCacheNamesReturn>(___r);
        }
        /// <summary> 
        /// Fetches cache entry. 
        /// </summary>
        public async Task<RequestCachedResponseReturn> RequestCachedResponse(RequestCachedResponseParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CacheStorage.requestCachedResponse", parameters, sessionId);
            return Convert<RequestCachedResponseReturn>(___r);
        }
        /// <summary> 
        /// Requests data from cache. 
        /// </summary>
        public async Task<RequestEntriesReturn> RequestEntries(RequestEntriesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("CacheStorage.requestEntries", parameters, sessionId);
            return Convert<RequestEntriesReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class DataEntryType
        {
            
            /// <summary> Request URL. </summary>
            public string requestURL;
            /// <summary> Request method. </summary>
            public string requestMethod;
            /// <summary> Request headers </summary>
            public object[] requestHeaders;
            /// <summary> Number of seconds since epoch. </summary>
            public double responseTime;
            /// <summary> HTTP response status code. </summary>
            public int responseStatus;
            /// <summary> HTTP response status text. </summary>
            public string responseStatusText;
            /// <summary> HTTP response type </summary>
            public string responseType;
            /// <summary> Response headers </summary>
            public object[] responseHeaders;
        }
        public class CacheType
        {
            
            /// <summary> An opaque unique id of the cache. </summary>
            public string cacheId;
            /// <summary> Security origin of the cache. </summary>
            public string securityOrigin;
            /// <summary> The name of the cache. </summary>
            public string cacheName;
        }
        public class HeaderType
        {
            
            /// <summary>  </summary>
            public string name;
            /// <summary>  </summary>
            public string value;
        }
        public class CachedResponseType
        {
            
            /// <summary> Entry content, base64-encoded. (Encoded as a base64 string when passed over JSON) </summary>
            public string body;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class DeleteCacheParameters
        {
            
            /// <summary> [Require] Id of cache for deletion. </summary>
            public string cacheId;
        }
        public class DeleteEntryParameters
        {
            
            /// <summary> [Require] Id of cache where the entry will be deleted. </summary>
            public string cacheId;
            /// <summary> [Require] URL spec of the request. </summary>
            public string request;
        }
        public class RequestCacheNamesParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string securityOrigin;
        }
        public class RequestCachedResponseParameters
        {
            
            /// <summary> [Require] Id of cache that contains the entry. </summary>
            public string cacheId;
            /// <summary> [Require] URL spec of the request. </summary>
            public string requestURL;
            /// <summary> [Require] headers of the request. </summary>
            public object[] requestHeaders;
        }
        public class RequestEntriesParameters
        {
            
            /// <summary> [Require] ID of cache to get entries from. </summary>
            public string cacheId;
            /// <summary> [Optional] Number of records to skip. </summary>
            public int skipCount;
            /// <summary> [Optional] Number of records to fetch. </summary>
            public int pageSize;
            /// <summary> [Optional] If present, only return the entries containing this substring in the path </summary>
            public string pathFilter;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class RequestCacheNamesReturn
        {
            
            /// <summary> Caches for the security origin. </summary>
            public object[] caches;
        }
        public class RequestCachedResponseReturn
        {
            
            /// <summary> Response read from the cache. </summary>
            public CacheStorage.CachedResponseType response;
        }
        public class RequestEntriesReturn
        {
            
            /// <summary> Array of object store data entries. </summary>
            public object[] cacheDataEntries;
            /// <summary> Count of returned entries from this storage. If pathFilter is empty, itis the count of all entries from this storage. </summary>
            public double returnCount;
        }
    }
    
    public class Cast : DomainBase
    {
        public Cast(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> This is fired whenever the list of available sinks changes. A sink is adevice or a software surface that you can cast to. </summary>
        /// <returns> remove handler </returns>
        public Action OnSinksUpdated(Action<OnSinksUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnSinksUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Cast.sinksUpdated" : $"Cast.sinksUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> This is fired whenever the outstanding issue/error message changes.|issueMessage| is empty if there is no issue. </summary>
        /// <returns> remove handler </returns>
        public Action OnIssueUpdated(Action<OnIssueUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnIssueUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Cast.issueUpdated" : $"Cast.issueUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Starts observing for sinks that can be used for tab mirroring, and if set,sinks compatible with |presentationUrl| as well. When sinks are found, a|sinksUpdated| event is fired.Also starts observing for issue messages. When an issue is added or removed,an |issueUpdated| event is fired. 
        /// </summary>
        public async Task Enable(EnableParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Cast.enable", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Stops observing for sinks and issues. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Cast.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets a sink to be used when the web page requests the browser to choose asink via Presentation API, Remote Playback API, or Cast SDK. 
        /// </summary>
        public async Task SetSinkToUse(SetSinkToUseParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Cast.setSinkToUse", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Starts mirroring the desktop to the sink. 
        /// </summary>
        public async Task StartDesktopMirroring(StartDesktopMirroringParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Cast.startDesktopMirroring", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Starts mirroring the tab to the sink. 
        /// </summary>
        public async Task StartTabMirroring(StartTabMirroringParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Cast.startTabMirroring", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Stops the active Cast session on the sink. 
        /// </summary>
        public async Task StopCasting(StopCastingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Cast.stopCasting", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class SinkType
        {
            
            /// <summary>  </summary>
            public string name;
            /// <summary>  </summary>
            public string id;
            /// <summary> Text describing the current session. Present only if there is an activesession on the sink. </summary>
            public string session;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnSinksUpdatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public object[] sinks;
        }
        public class OnIssueUpdatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string issueMessage;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class EnableParameters
        {
            
            /// <summary> [Optional]  </summary>
            public string presentationUrl;
        }
        public class SetSinkToUseParameters
        {
            
            /// <summary> [Require]  </summary>
            public string sinkName;
        }
        public class StartDesktopMirroringParameters
        {
            
            /// <summary> [Require]  </summary>
            public string sinkName;
        }
        public class StartTabMirroringParameters
        {
            
            /// <summary> [Require]  </summary>
            public string sinkName;
        }
        public class StopCastingParameters
        {
            
            /// <summary> [Require]  </summary>
            public string sinkName;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class DOM : DomainBase
    {
        public DOM(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Fired when `Element`'s attribute is modified. </summary>
        /// <returns> remove handler </returns>
        public Action OnAttributeModified(Action<OnAttributeModifiedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAttributeModifiedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.attributeModified" : $"DOM.attributeModified.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when `Element`'s attribute is removed. </summary>
        /// <returns> remove handler </returns>
        public Action OnAttributeRemoved(Action<OnAttributeRemovedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAttributeRemovedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.attributeRemoved" : $"DOM.attributeRemoved.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Mirrors `DOMCharacterDataModified` event. </summary>
        /// <returns> remove handler </returns>
        public Action OnCharacterDataModified(Action<OnCharacterDataModifiedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnCharacterDataModifiedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.characterDataModified" : $"DOM.characterDataModified.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when `Container`'s child node count has changed. </summary>
        /// <returns> remove handler </returns>
        public Action OnChildNodeCountUpdated(Action<OnChildNodeCountUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnChildNodeCountUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.childNodeCountUpdated" : $"DOM.childNodeCountUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Mirrors `DOMNodeInserted` event. </summary>
        /// <returns> remove handler </returns>
        public Action OnChildNodeInserted(Action<OnChildNodeInsertedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnChildNodeInsertedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.childNodeInserted" : $"DOM.childNodeInserted.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Mirrors `DOMNodeRemoved` event. </summary>
        /// <returns> remove handler </returns>
        public Action OnChildNodeRemoved(Action<OnChildNodeRemovedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnChildNodeRemovedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.childNodeRemoved" : $"DOM.childNodeRemoved.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Called when distribution is changed. </summary>
        /// <returns> remove handler </returns>
        public Action OnDistributedNodesUpdated(Action<OnDistributedNodesUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDistributedNodesUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.distributedNodesUpdated" : $"DOM.distributedNodesUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when `Document` has been totally updated. Node ids are no longer valid. </summary>
        /// <returns> remove handler </returns>
        public Action OnDocumentUpdated(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.documentUpdated" : $"DOM.documentUpdated.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        
        /// <summary> Fired when `Element`'s inline style is modified via a CSS property modification. </summary>
        /// <returns> remove handler </returns>
        public Action OnInlineStyleInvalidated(Action<OnInlineStyleInvalidatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnInlineStyleInvalidatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.inlineStyleInvalidated" : $"DOM.inlineStyleInvalidated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Called when a pseudo element is added to an element. </summary>
        /// <returns> remove handler </returns>
        public Action OnPseudoElementAdded(Action<OnPseudoElementAddedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnPseudoElementAddedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.pseudoElementAdded" : $"DOM.pseudoElementAdded.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Called when a pseudo element is removed from an element. </summary>
        /// <returns> remove handler </returns>
        public Action OnPseudoElementRemoved(Action<OnPseudoElementRemovedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnPseudoElementRemovedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.pseudoElementRemoved" : $"DOM.pseudoElementRemoved.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when backend wants to provide client with the missing DOM structure. This happens uponmost of the calls requesting node ids. </summary>
        /// <returns> remove handler </returns>
        public Action OnSetChildNodes(Action<OnSetChildNodesParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnSetChildNodesParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.setChildNodes" : $"DOM.setChildNodes.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Called when shadow root is popped from the element. </summary>
        /// <returns> remove handler </returns>
        public Action OnShadowRootPopped(Action<OnShadowRootPoppedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnShadowRootPoppedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.shadowRootPopped" : $"DOM.shadowRootPopped.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Called when shadow root is pushed into the element. </summary>
        /// <returns> remove handler </returns>
        public Action OnShadowRootPushed(Action<OnShadowRootPushedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnShadowRootPushedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOM.shadowRootPushed" : $"DOM.shadowRootPushed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Collects class names for the node with given id and all of it's child nodes. 
        /// </summary>
        public async Task<CollectClassNamesFromSubtreeReturn> CollectClassNamesFromSubtree(CollectClassNamesFromSubtreeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.collectClassNamesFromSubtree", parameters, sessionId);
            return Convert<CollectClassNamesFromSubtreeReturn>(___r);
        }
        /// <summary> 
        /// Creates a deep copy of the specified node and places it into the target container before thegiven anchor. 
        /// </summary>
        public async Task<CopyToReturn> CopyTo(CopyToParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.copyTo", parameters, sessionId);
            return Convert<CopyToReturn>(___r);
        }
        /// <summary> 
        /// Describes node given its id, does not require domain to be enabled. Does not start tracking anyobjects, can be used for automation. 
        /// </summary>
        public async Task<DescribeNodeReturn> DescribeNode(DescribeNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.describeNode", parameters, sessionId);
            return Convert<DescribeNodeReturn>(___r);
        }
        /// <summary> 
        /// Scrolls the specified rect of the given node into view if not already visible.Note: exactly one between nodeId, backendNodeId and objectId should be passedto identify the node. 
        /// </summary>
        public async Task ScrollIntoViewIfNeeded(ScrollIntoViewIfNeededParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.scrollIntoViewIfNeeded", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables DOM agent for the given page. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Discards search results from the session with the given id. `getSearchResults` should no longerbe called for that search. 
        /// </summary>
        public async Task DiscardSearchResults(DiscardSearchResultsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.discardSearchResults", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables DOM agent for the given page. 
        /// </summary>
        public async Task Enable(EnableParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.enable", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Focuses the given element. 
        /// </summary>
        public async Task Focus(FocusParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.focus", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns attributes for the specified node. 
        /// </summary>
        public async Task<GetAttributesReturn> GetAttributes(GetAttributesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getAttributes", parameters, sessionId);
            return Convert<GetAttributesReturn>(___r);
        }
        /// <summary> 
        /// Returns boxes for the given node. 
        /// </summary>
        public async Task<GetBoxModelReturn> GetBoxModel(GetBoxModelParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getBoxModel", parameters, sessionId);
            return Convert<GetBoxModelReturn>(___r);
        }
        /// <summary> 
        /// Returns quads that describe node position on the page. This methodmight return multiple quads for inline nodes. 
        /// </summary>
        public async Task<GetContentQuadsReturn> GetContentQuads(GetContentQuadsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getContentQuads", parameters, sessionId);
            return Convert<GetContentQuadsReturn>(___r);
        }
        /// <summary> 
        /// Returns the root DOM node (and optionally the subtree) to the caller. 
        /// </summary>
        public async Task<GetDocumentReturn> GetDocument(GetDocumentParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getDocument", parameters, sessionId);
            return Convert<GetDocumentReturn>(___r);
        }
        /// <summary> 
        /// Returns the root DOM node (and optionally the subtree) to the caller.Deprecated, as it is not designed to work well with the rest of the DOM agent.Use DOMSnapshot.captureSnapshot instead. 
        /// </summary>
        public async Task<GetFlattenedDocumentReturn> GetFlattenedDocument(GetFlattenedDocumentParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getFlattenedDocument", parameters, sessionId);
            return Convert<GetFlattenedDocumentReturn>(___r);
        }
        /// <summary> 
        /// Finds nodes with a given computed style in a subtree. 
        /// </summary>
        public async Task<GetNodesForSubtreeByStyleReturn> GetNodesForSubtreeByStyle(GetNodesForSubtreeByStyleParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getNodesForSubtreeByStyle", parameters, sessionId);
            return Convert<GetNodesForSubtreeByStyleReturn>(___r);
        }
        /// <summary> 
        /// Returns node id at given location. Depending on whether DOM domain is enabled, nodeId iseither returned or not. 
        /// </summary>
        public async Task<GetNodeForLocationReturn> GetNodeForLocation(GetNodeForLocationParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getNodeForLocation", parameters, sessionId);
            return Convert<GetNodeForLocationReturn>(___r);
        }
        /// <summary> 
        /// Returns node's HTML markup. 
        /// </summary>
        public async Task<GetOuterHTMLReturn> GetOuterHTML(GetOuterHTMLParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getOuterHTML", parameters, sessionId);
            return Convert<GetOuterHTMLReturn>(___r);
        }
        /// <summary> 
        /// Returns the id of the nearest ancestor that is a relayout boundary. 
        /// </summary>
        public async Task<GetRelayoutBoundaryReturn> GetRelayoutBoundary(GetRelayoutBoundaryParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getRelayoutBoundary", parameters, sessionId);
            return Convert<GetRelayoutBoundaryReturn>(___r);
        }
        /// <summary> 
        /// Returns search results from given `fromIndex` to given `toIndex` from the search with the givenidentifier. 
        /// </summary>
        public async Task<GetSearchResultsReturn> GetSearchResults(GetSearchResultsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getSearchResults", parameters, sessionId);
            return Convert<GetSearchResultsReturn>(___r);
        }
        /// <summary> 
        /// Hides any highlight. 
        /// </summary>
        public async Task HideHighlight(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.hideHighlight", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Highlights DOM node. 
        /// </summary>
        public async Task HighlightNode(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.highlightNode", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Highlights given rectangle. 
        /// </summary>
        public async Task HighlightRect(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.highlightRect", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Marks last undoable state. 
        /// </summary>
        public async Task MarkUndoableState(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.markUndoableState", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Moves node into the new container, places it before the given anchor. 
        /// </summary>
        public async Task<MoveToReturn> MoveTo(MoveToParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.moveTo", parameters, sessionId);
            return Convert<MoveToReturn>(___r);
        }
        /// <summary> 
        /// Searches for a given string in the DOM tree. Use `getSearchResults` to access search results or`cancelSearch` to end this search session. 
        /// </summary>
        public async Task<PerformSearchReturn> PerformSearch(PerformSearchParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.performSearch", parameters, sessionId);
            return Convert<PerformSearchReturn>(___r);
        }
        /// <summary> 
        /// Requests that the node is sent to the caller given its path. // FIXME, use XPath 
        /// </summary>
        public async Task<PushNodeByPathToFrontendReturn> PushNodeByPathToFrontend(PushNodeByPathToFrontendParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.pushNodeByPathToFrontend", parameters, sessionId);
            return Convert<PushNodeByPathToFrontendReturn>(___r);
        }
        /// <summary> 
        /// Requests that a batch of nodes is sent to the caller given their backend node ids. 
        /// </summary>
        public async Task<PushNodesByBackendIdsToFrontendReturn> PushNodesByBackendIdsToFrontend(PushNodesByBackendIdsToFrontendParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.pushNodesByBackendIdsToFrontend", parameters, sessionId);
            return Convert<PushNodesByBackendIdsToFrontendReturn>(___r);
        }
        /// <summary> 
        /// Executes `querySelector` on a given node. 
        /// </summary>
        public async Task<QuerySelectorReturn> QuerySelector(QuerySelectorParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.querySelector", parameters, sessionId);
            return Convert<QuerySelectorReturn>(___r);
        }
        /// <summary> 
        /// Executes `querySelectorAll` on a given node. 
        /// </summary>
        public async Task<QuerySelectorAllReturn> QuerySelectorAll(QuerySelectorAllParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.querySelectorAll", parameters, sessionId);
            return Convert<QuerySelectorAllReturn>(___r);
        }
        /// <summary> 
        /// Re-does the last undone action. 
        /// </summary>
        public async Task Redo(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.redo", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Removes attribute with given name from an element with given id. 
        /// </summary>
        public async Task RemoveAttribute(RemoveAttributeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.removeAttribute", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Removes node with given id. 
        /// </summary>
        public async Task RemoveNode(RemoveNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.removeNode", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests that children of the node with given id are returned to the caller in form of`setChildNodes` events where not only immediate children are retrieved, but all children down tothe specified depth. 
        /// </summary>
        public async Task RequestChildNodes(RequestChildNodesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.requestChildNodes", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests that the node is sent to the caller given the JavaScript node object reference. Allnodes that form the path from the node to the root are also sent to the client as a series of`setChildNodes` notifications. 
        /// </summary>
        public async Task<RequestNodeReturn> RequestNode(RequestNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.requestNode", parameters, sessionId);
            return Convert<RequestNodeReturn>(___r);
        }
        /// <summary> 
        /// Resolves the JavaScript node object for a given NodeId or BackendNodeId. 
        /// </summary>
        public async Task<ResolveNodeReturn> ResolveNode(ResolveNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.resolveNode", parameters, sessionId);
            return Convert<ResolveNodeReturn>(___r);
        }
        /// <summary> 
        /// Sets attribute for an element with given id. 
        /// </summary>
        public async Task SetAttributeValue(SetAttributeValueParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.setAttributeValue", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets attributes on element with given id. This method is useful when user edits some existingattribute value and types in several attribute name/value pairs. 
        /// </summary>
        public async Task SetAttributesAsText(SetAttributesAsTextParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.setAttributesAsText", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets files for the given file input element. 
        /// </summary>
        public async Task SetFileInputFiles(SetFileInputFilesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.setFileInputFiles", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets if stack traces should be captured for Nodes. See `Node.getNodeStackTraces`. Default is disabled. 
        /// </summary>
        public async Task SetNodeStackTracesEnabled(SetNodeStackTracesEnabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.setNodeStackTracesEnabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Gets stack traces associated with a Node. As of now, only provides stack trace for Node creation. 
        /// </summary>
        public async Task<GetNodeStackTracesReturn> GetNodeStackTraces(GetNodeStackTracesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getNodeStackTraces", parameters, sessionId);
            return Convert<GetNodeStackTracesReturn>(___r);
        }
        /// <summary> 
        /// Returns file information for the givenFile wrapper. 
        /// </summary>
        public async Task<GetFileInfoReturn> GetFileInfo(GetFileInfoParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getFileInfo", parameters, sessionId);
            return Convert<GetFileInfoReturn>(___r);
        }
        /// <summary> 
        /// Enables console to refer to the node with given id via $x (see Command Line API for more details$x functions). 
        /// </summary>
        public async Task SetInspectedNode(SetInspectedNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.setInspectedNode", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets node name for a node with given id. 
        /// </summary>
        public async Task<SetNodeNameReturn> SetNodeName(SetNodeNameParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.setNodeName", parameters, sessionId);
            return Convert<SetNodeNameReturn>(___r);
        }
        /// <summary> 
        /// Sets node value for a node with given id. 
        /// </summary>
        public async Task SetNodeValue(SetNodeValueParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.setNodeValue", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets node HTML markup, returns new node id. 
        /// </summary>
        public async Task SetOuterHTML(SetOuterHTMLParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.setOuterHTML", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Undoes the last performed action. 
        /// </summary>
        public async Task Undo(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.undo", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns iframe node that owns iframe with the given domain. 
        /// </summary>
        public async Task<GetFrameOwnerReturn> GetFrameOwner(GetFrameOwnerParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getFrameOwner", parameters, sessionId);
            return Convert<GetFrameOwnerReturn>(___r);
        }
        /// <summary> 
        /// Returns the container of the given node based on container query conditions.If containerName is given, it will find the nearest container with a matching name;otherwise it will find the nearest container regardless of its container name. 
        /// </summary>
        public async Task<GetContainerForNodeReturn> GetContainerForNode(GetContainerForNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getContainerForNode", parameters, sessionId);
            return Convert<GetContainerForNodeReturn>(___r);
        }
        /// <summary> 
        /// Returns the descendants of a container query container that havecontainer queries against this container. 
        /// </summary>
        public async Task<GetQueryingDescendantsForContainerReturn> GetQueryingDescendantsForContainer(GetQueryingDescendantsForContainerParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOM.getQueryingDescendantsForContainer", parameters, sessionId);
            return Convert<GetQueryingDescendantsForContainerReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class BackendNodeType
        {
            
            /// <summary> `Node`'s nodeType. </summary>
            public int nodeType;
            /// <summary> `Node`'s nodeName. </summary>
            public string nodeName;
            /// <summary>  </summary>
            public int backendNodeId;
        }
        public class NodeType
        {
            
            /// <summary> Node identifier that is passed into the rest of the DOM messages as the `nodeId`. Backendwill only push node with given `id` once. It is aware of all requested nodes and will onlyfire DOM events for nodes known to the client. </summary>
            public int nodeId;
            /// <summary> The id of the parent node if any. </summary>
            public int parentId;
            /// <summary> The BackendNodeId for this node. </summary>
            public int backendNodeId;
            /// <summary> `Node`'s nodeType. </summary>
            public int nodeType;
            /// <summary> `Node`'s nodeName. </summary>
            public string nodeName;
            /// <summary> `Node`'s localName. </summary>
            public string localName;
            /// <summary> `Node`'s nodeValue. </summary>
            public string nodeValue;
            /// <summary> Child count for `Container` nodes. </summary>
            public int childNodeCount;
            /// <summary> Child nodes of this node when requested with children. </summary>
            public object[] children;
            /// <summary> Attributes of the `Element` node in the form of flat array `[name1, value1, name2, value2]`. </summary>
            public object[] attributes;
            /// <summary> Document URL that `Document` or `FrameOwner` node points to. </summary>
            public string documentURL;
            /// <summary> Base URL that `Document` or `FrameOwner` node uses for URL completion. </summary>
            public string baseURL;
            /// <summary> `DocumentType`'s publicId. </summary>
            public string publicId;
            /// <summary> `DocumentType`'s systemId. </summary>
            public string systemId;
            /// <summary> `DocumentType`'s internalSubset. </summary>
            public string internalSubset;
            /// <summary> `Document`'s XML version in case of XML documents. </summary>
            public string xmlVersion;
            /// <summary> `Attr`'s name. </summary>
            public string name;
            /// <summary> `Attr`'s value. </summary>
            public string value;
            /// <summary> Pseudo element type for this node. </summary>
            public string pseudoType;
            /// <summary> Shadow root type. </summary>
            public string shadowRootType;
            /// <summary> Frame ID for frame owner elements. </summary>
            public string frameId;
            /// <summary> Content document for frame owner elements. </summary>
            public DOM.NodeType contentDocument;
            /// <summary> Shadow root list for given element host. </summary>
            public object[] shadowRoots;
            /// <summary> Content document fragment for template elements. </summary>
            public DOM.NodeType templateContent;
            /// <summary> Pseudo elements associated with this node. </summary>
            public object[] pseudoElements;
            /// <summary> Deprecated, as the HTML Imports API has been removed (crbug.com/937746).This property used to return the imported document for the HTMLImport links.The property is always undefined now. </summary>
            public DOM.NodeType importedDocument;
            /// <summary> Distributed nodes for given insertion point. </summary>
            public object[] distributedNodes;
            /// <summary> Whether the node is SVG. </summary>
            public bool isSVG;
            /// <summary>  </summary>
            public string compatibilityMode;
        }
        public class RGBAType
        {
            
            /// <summary> The red component, in the [0-255] range. </summary>
            public int r;
            /// <summary> The green component, in the [0-255] range. </summary>
            public int g;
            /// <summary> The blue component, in the [0-255] range. </summary>
            public int b;
            /// <summary> The alpha component, in the [0-1] range (default: 1). </summary>
            public double a;
        }
        public class BoxModelType
        {
            
            /// <summary> Content box </summary>
            public object[] content;
            /// <summary> Padding box </summary>
            public object[] padding;
            /// <summary> Border box </summary>
            public object[] border;
            /// <summary> Margin box </summary>
            public object[] margin;
            /// <summary> Node width </summary>
            public int width;
            /// <summary> Node height </summary>
            public int height;
            /// <summary> Shape outside coordinates </summary>
            public DOM.ShapeOutsideInfoType shapeOutside;
        }
        public class ShapeOutsideInfoType
        {
            
            /// <summary> Shape bounds </summary>
            public object[] bounds;
            /// <summary> Shape coordinate details </summary>
            public object[] shape;
            /// <summary> Margin shape bounds </summary>
            public object[] marginShape;
        }
        public class RectType
        {
            
            /// <summary> X coordinate </summary>
            public double x;
            /// <summary> Y coordinate </summary>
            public double y;
            /// <summary> Rectangle width </summary>
            public double width;
            /// <summary> Rectangle height </summary>
            public double height;
        }
        public class CSSComputedStylePropertyType
        {
            
            /// <summary> Computed style property name. </summary>
            public string name;
            /// <summary> Computed style property value. </summary>
            public string value;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnAttributeModifiedParameters
        {
            
            /// <summary> [Require] Id of the node that has changed. </summary>
            public int nodeId;
            /// <summary> [Require] Attribute name. </summary>
            public string name;
            /// <summary> [Require] Attribute value. </summary>
            public string value;
        }
        public class OnAttributeRemovedParameters
        {
            
            /// <summary> [Require] Id of the node that has changed. </summary>
            public int nodeId;
            /// <summary> [Require] A ttribute name. </summary>
            public string name;
        }
        public class OnCharacterDataModifiedParameters
        {
            
            /// <summary> [Require] Id of the node that has changed. </summary>
            public int nodeId;
            /// <summary> [Require] New text value. </summary>
            public string characterData;
        }
        public class OnChildNodeCountUpdatedParameters
        {
            
            /// <summary> [Require] Id of the node that has changed. </summary>
            public int nodeId;
            /// <summary> [Require] New node count. </summary>
            public int childNodeCount;
        }
        public class OnChildNodeInsertedParameters
        {
            
            /// <summary> [Require] Id of the node that has changed. </summary>
            public int parentNodeId;
            /// <summary> [Require] If of the previous siblint. </summary>
            public int previousNodeId;
            /// <summary> [Require] Inserted node data. </summary>
            public DOM.NodeType node;
        }
        public class OnChildNodeRemovedParameters
        {
            
            /// <summary> [Require] Parent id. </summary>
            public int parentNodeId;
            /// <summary> [Require] Id of the node that has been removed. </summary>
            public int nodeId;
        }
        public class OnDistributedNodesUpdatedParameters
        {
            
            /// <summary> [Require] Insertion point where distributed nodes were updated. </summary>
            public int insertionPointId;
            /// <summary> [Require] Distributed nodes for given insertion point. </summary>
            public object[] distributedNodes;
        }
        public class OnInlineStyleInvalidatedParameters
        {
            
            /// <summary> [Require] Ids of the nodes for which the inline styles have been invalidated. </summary>
            public object[] nodeIds;
        }
        public class OnPseudoElementAddedParameters
        {
            
            /// <summary> [Require] Pseudo element's parent element id. </summary>
            public int parentId;
            /// <summary> [Require] The added pseudo element. </summary>
            public DOM.NodeType pseudoElement;
        }
        public class OnPseudoElementRemovedParameters
        {
            
            /// <summary> [Require] Pseudo element's parent element id. </summary>
            public int parentId;
            /// <summary> [Require] The removed pseudo element id. </summary>
            public int pseudoElementId;
        }
        public class OnSetChildNodesParameters
        {
            
            /// <summary> [Require] Parent node id to populate with children. </summary>
            public int parentId;
            /// <summary> [Require] Child nodes array. </summary>
            public object[] nodes;
        }
        public class OnShadowRootPoppedParameters
        {
            
            /// <summary> [Require] Host element id. </summary>
            public int hostId;
            /// <summary> [Require] Shadow root id. </summary>
            public int rootId;
        }
        public class OnShadowRootPushedParameters
        {
            
            /// <summary> [Require] Host element id. </summary>
            public int hostId;
            /// <summary> [Require] Shadow root. </summary>
            public DOM.NodeType root;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class CollectClassNamesFromSubtreeParameters
        {
            
            /// <summary> [Require] Id of the node to collect class names. </summary>
            public int nodeId;
        }
        public class CopyToParameters
        {
            
            /// <summary> [Require] Id of the node to copy. </summary>
            public int nodeId;
            /// <summary> [Require] Id of the element to drop the copy into. </summary>
            public int targetNodeId;
            /// <summary> [Optional] Drop the copy before this node (if absent, the copy becomes the last child of`targetNodeId`). </summary>
            public int insertBeforeNodeId;
        }
        public class DescribeNodeParameters
        {
            
            /// <summary> [Optional] Identifier of the node. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper. </summary>
            public string objectId;
            /// <summary> [Optional] The maximum depth at which children should be retrieved, defaults to 1. Use -1 for theentire subtree or provide an integer larger than 0. </summary>
            public int depth;
            /// <summary> [Optional] Whether or not iframes and shadow roots should be traversed when returning the subtree(default is false). </summary>
            public bool pierce;
        }
        public class ScrollIntoViewIfNeededParameters
        {
            
            /// <summary> [Optional] Identifier of the node. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper. </summary>
            public string objectId;
            /// <summary> [Optional] The rect to be scrolled into view, relative to the node's border box, in CSS pixels.When omitted, center of the node will be used, similar to Element.scrollIntoView. </summary>
            public DOM.RectType rect;
        }
        public class DiscardSearchResultsParameters
        {
            
            /// <summary> [Require] Unique search session identifier. </summary>
            public string searchId;
        }
        public class EnableParameters
        {
            
            /// <summary> [Optional] Whether to include whitespaces in the children array of returned Nodes. </summary>
            public string includeWhitespace;
        }
        public class FocusParameters
        {
            
            /// <summary> [Optional] Identifier of the node. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper. </summary>
            public string objectId;
        }
        public class GetAttributesParameters
        {
            
            /// <summary> [Require] Id of the node to retrieve attibutes for. </summary>
            public int nodeId;
        }
        public class GetBoxModelParameters
        {
            
            /// <summary> [Optional] Identifier of the node. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper. </summary>
            public string objectId;
        }
        public class GetContentQuadsParameters
        {
            
            /// <summary> [Optional] Identifier of the node. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper. </summary>
            public string objectId;
        }
        public class GetDocumentParameters
        {
            
            /// <summary> [Optional] The maximum depth at which children should be retrieved, defaults to 1. Use -1 for theentire subtree or provide an integer larger than 0. </summary>
            public int depth;
            /// <summary> [Optional] Whether or not iframes and shadow roots should be traversed when returning the subtree(default is false). </summary>
            public bool pierce;
        }
        public class GetFlattenedDocumentParameters
        {
            
            /// <summary> [Optional] The maximum depth at which children should be retrieved, defaults to 1. Use -1 for theentire subtree or provide an integer larger than 0. </summary>
            public int depth;
            /// <summary> [Optional] Whether or not iframes and shadow roots should be traversed when returning the subtree(default is false). </summary>
            public bool pierce;
        }
        public class GetNodesForSubtreeByStyleParameters
        {
            
            /// <summary> [Require] Node ID pointing to the root of a subtree. </summary>
            public int nodeId;
            /// <summary> [Require] The style to filter nodes by (includes nodes if any of properties matches). </summary>
            public object[] computedStyles;
            /// <summary> [Optional] Whether or not iframes and shadow roots in the same target should be traversed when returning theresults (default is false). </summary>
            public bool pierce;
        }
        public class GetNodeForLocationParameters
        {
            
            /// <summary> [Require] X coordinate. </summary>
            public int x;
            /// <summary> [Require] Y coordinate. </summary>
            public int y;
            /// <summary> [Optional] False to skip to the nearest non-UA shadow root ancestor (default: false). </summary>
            public bool includeUserAgentShadowDOM;
            /// <summary> [Optional] Whether to ignore pointer-events: none on elements and hit test them. </summary>
            public bool ignorePointerEventsNone;
        }
        public class GetOuterHTMLParameters
        {
            
            /// <summary> [Optional] Identifier of the node. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper. </summary>
            public string objectId;
        }
        public class GetRelayoutBoundaryParameters
        {
            
            /// <summary> [Require] Id of the node. </summary>
            public int nodeId;
        }
        public class GetSearchResultsParameters
        {
            
            /// <summary> [Require] Unique search session identifier. </summary>
            public string searchId;
            /// <summary> [Require] Start index of the search result to be returned. </summary>
            public int fromIndex;
            /// <summary> [Require] End index of the search result to be returned. </summary>
            public int toIndex;
        }
        public class MoveToParameters
        {
            
            /// <summary> [Require] Id of the node to move. </summary>
            public int nodeId;
            /// <summary> [Require] Id of the element to drop the moved node into. </summary>
            public int targetNodeId;
            /// <summary> [Optional] Drop node before this one (if absent, the moved node becomes the last child of`targetNodeId`). </summary>
            public int insertBeforeNodeId;
        }
        public class PerformSearchParameters
        {
            
            /// <summary> [Require] Plain text or query selector or XPath search query. </summary>
            public string query;
            /// <summary> [Optional] True to search in user agent shadow DOM. </summary>
            public bool includeUserAgentShadowDOM;
        }
        public class PushNodeByPathToFrontendParameters
        {
            
            /// <summary> [Require] Path to node in the proprietary format. </summary>
            public string path;
        }
        public class PushNodesByBackendIdsToFrontendParameters
        {
            
            /// <summary> [Require] The array of backend node ids. </summary>
            public object[] backendNodeIds;
        }
        public class QuerySelectorParameters
        {
            
            /// <summary> [Require] Id of the node to query upon. </summary>
            public int nodeId;
            /// <summary> [Require] Selector string. </summary>
            public string selector;
        }
        public class QuerySelectorAllParameters
        {
            
            /// <summary> [Require] Id of the node to query upon. </summary>
            public int nodeId;
            /// <summary> [Require] Selector string. </summary>
            public string selector;
        }
        public class RemoveAttributeParameters
        {
            
            /// <summary> [Require] Id of the element to remove attribute from. </summary>
            public int nodeId;
            /// <summary> [Require] Name of the attribute to remove. </summary>
            public string name;
        }
        public class RemoveNodeParameters
        {
            
            /// <summary> [Require] Id of the node to remove. </summary>
            public int nodeId;
        }
        public class RequestChildNodesParameters
        {
            
            /// <summary> [Require] Id of the node to get children for. </summary>
            public int nodeId;
            /// <summary> [Optional] The maximum depth at which children should be retrieved, defaults to 1. Use -1 for theentire subtree or provide an integer larger than 0. </summary>
            public int depth;
            /// <summary> [Optional] Whether or not iframes and shadow roots should be traversed when returning the sub-tree(default is false). </summary>
            public bool pierce;
        }
        public class RequestNodeParameters
        {
            
            /// <summary> [Require] JavaScript object id to convert into node. </summary>
            public string objectId;
        }
        public class ResolveNodeParameters
        {
            
            /// <summary> [Optional] Id of the node to resolve. </summary>
            public int nodeId;
            /// <summary> [Optional] Backend identifier of the node to resolve. </summary>
            public int backendNodeId;
            /// <summary> [Optional] Symbolic group name that can be used to release multiple objects. </summary>
            public string objectGroup;
            /// <summary> [Optional] Execution context in which to resolve the node. </summary>
            public int executionContextId;
        }
        public class SetAttributeValueParameters
        {
            
            /// <summary> [Require] Id of the element to set attribute for. </summary>
            public int nodeId;
            /// <summary> [Require] Attribute name. </summary>
            public string name;
            /// <summary> [Require] Attribute value. </summary>
            public string value;
        }
        public class SetAttributesAsTextParameters
        {
            
            /// <summary> [Require] Id of the element to set attributes for. </summary>
            public int nodeId;
            /// <summary> [Require] Text with a number of attributes. Will parse this text using HTML parser. </summary>
            public string text;
            /// <summary> [Optional] Attribute name to replace with new attributes derived from text in case text parsedsuccessfully. </summary>
            public string name;
        }
        public class SetFileInputFilesParameters
        {
            
            /// <summary> [Require] Array of file paths to set. </summary>
            public object[] files;
            /// <summary> [Optional] Identifier of the node. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node wrapper. </summary>
            public string objectId;
        }
        public class SetNodeStackTracesEnabledParameters
        {
            
            /// <summary> [Require] Enable or disable. </summary>
            public bool enable;
        }
        public class GetNodeStackTracesParameters
        {
            
            /// <summary> [Require] Id of the node to get stack traces for. </summary>
            public int nodeId;
        }
        public class GetFileInfoParameters
        {
            
            /// <summary> [Require] JavaScript object id of the node wrapper. </summary>
            public string objectId;
        }
        public class SetInspectedNodeParameters
        {
            
            /// <summary> [Require] DOM node id to be accessible by means of $x command line API. </summary>
            public int nodeId;
        }
        public class SetNodeNameParameters
        {
            
            /// <summary> [Require] Id of the node to set name for. </summary>
            public int nodeId;
            /// <summary> [Require] New node's name. </summary>
            public string name;
        }
        public class SetNodeValueParameters
        {
            
            /// <summary> [Require] Id of the node to set value for. </summary>
            public int nodeId;
            /// <summary> [Require] New node's value. </summary>
            public string value;
        }
        public class SetOuterHTMLParameters
        {
            
            /// <summary> [Require] Id of the node to set markup for. </summary>
            public int nodeId;
            /// <summary> [Require] Outer HTML markup to set. </summary>
            public string outerHTML;
        }
        public class GetFrameOwnerParameters
        {
            
            /// <summary> [Require]  </summary>
            public string frameId;
        }
        public class GetContainerForNodeParameters
        {
            
            /// <summary> [Require]  </summary>
            public int nodeId;
            /// <summary> [Optional]  </summary>
            public string containerName;
        }
        public class GetQueryingDescendantsForContainerParameters
        {
            
            /// <summary> [Require] Id of the container node to find querying descendants from. </summary>
            public int nodeId;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class CollectClassNamesFromSubtreeReturn
        {
            
            /// <summary> Class name list. </summary>
            public object[] classNames;
        }
        public class CopyToReturn
        {
            
            /// <summary> Id of the node clone. </summary>
            public int nodeId;
        }
        public class DescribeNodeReturn
        {
            
            /// <summary> Node description. </summary>
            public DOM.NodeType node;
        }
        public class GetAttributesReturn
        {
            
            /// <summary> An interleaved array of node attribute names and values. </summary>
            public object[] attributes;
        }
        public class GetBoxModelReturn
        {
            
            /// <summary> Box model for the node. </summary>
            public DOM.BoxModelType model;
        }
        public class GetContentQuadsReturn
        {
            
            /// <summary> Quads that describe node layout relative to viewport. </summary>
            public object[] quads;
        }
        public class GetDocumentReturn
        {
            
            /// <summary> Resulting node. </summary>
            public DOM.NodeType root;
        }
        public class GetFlattenedDocumentReturn
        {
            
            /// <summary> Resulting node. </summary>
            public object[] nodes;
        }
        public class GetNodesForSubtreeByStyleReturn
        {
            
            /// <summary> Resulting nodes. </summary>
            public object[] nodeIds;
        }
        public class GetNodeForLocationReturn
        {
            
            /// <summary> Resulting node. </summary>
            public int backendNodeId;
            /// <summary> Frame this node belongs to. </summary>
            public string frameId;
            /// <summary> Id of the node at given coordinates, only when enabled and requested document. </summary>
            public int nodeId;
        }
        public class GetOuterHTMLReturn
        {
            
            /// <summary> Outer HTML markup. </summary>
            public string outerHTML;
        }
        public class GetRelayoutBoundaryReturn
        {
            
            /// <summary> Relayout boundary node id for the given node. </summary>
            public int nodeId;
        }
        public class GetSearchResultsReturn
        {
            
            /// <summary> Ids of the search result nodes. </summary>
            public object[] nodeIds;
        }
        public class MoveToReturn
        {
            
            /// <summary> New id of the moved node. </summary>
            public int nodeId;
        }
        public class PerformSearchReturn
        {
            
            /// <summary> Unique search session identifier. </summary>
            public string searchId;
            /// <summary> Number of search results. </summary>
            public int resultCount;
        }
        public class PushNodeByPathToFrontendReturn
        {
            
            /// <summary> Id of the node for given path. </summary>
            public int nodeId;
        }
        public class PushNodesByBackendIdsToFrontendReturn
        {
            
            /// <summary> The array of ids of pushed nodes that correspond to the backend ids specified inbackendNodeIds. </summary>
            public object[] nodeIds;
        }
        public class QuerySelectorReturn
        {
            
            /// <summary> Query selector result. </summary>
            public int nodeId;
        }
        public class QuerySelectorAllReturn
        {
            
            /// <summary> Query selector result. </summary>
            public object[] nodeIds;
        }
        public class RequestNodeReturn
        {
            
            /// <summary> Node id for given object. </summary>
            public int nodeId;
        }
        public class ResolveNodeReturn
        {
            
            /// <summary> JavaScript object wrapper for given node. </summary>
            public Runtime.RemoteObjectType @object;
        }
        public class GetNodeStackTracesReturn
        {
            
            /// <summary> Creation stack trace, if available. </summary>
            public Runtime.StackTraceType creation;
        }
        public class GetFileInfoReturn
        {
            
            /// <summary>  </summary>
            public string path;
        }
        public class SetNodeNameReturn
        {
            
            /// <summary> New node's id. </summary>
            public int nodeId;
        }
        public class GetFrameOwnerReturn
        {
            
            /// <summary> Resulting node. </summary>
            public int backendNodeId;
            /// <summary> Id of the node at given coordinates, only when enabled and requested document. </summary>
            public int nodeId;
        }
        public class GetContainerForNodeReturn
        {
            
            /// <summary> The container node for the given node, or null if not found. </summary>
            public int nodeId;
        }
        public class GetQueryingDescendantsForContainerReturn
        {
            
            /// <summary> Descendant nodes with container queries against the given container. </summary>
            public object[] nodeIds;
        }
    }
    
    public class DOMDebugger : DomainBase
    {
        public DOMDebugger(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Returns event listeners of the given object. 
        /// </summary>
        public async Task<GetEventListenersReturn> GetEventListeners(GetEventListenersParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.getEventListeners", parameters, sessionId);
            return Convert<GetEventListenersReturn>(___r);
        }
        /// <summary> 
        /// Removes DOM breakpoint that was set using `setDOMBreakpoint`. 
        /// </summary>
        public async Task RemoveDOMBreakpoint(RemoveDOMBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.removeDOMBreakpoint", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Removes breakpoint on particular DOM event. 
        /// </summary>
        public async Task RemoveEventListenerBreakpoint(RemoveEventListenerBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.removeEventListenerBreakpoint", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Removes breakpoint on particular native event. 
        /// </summary>
        public async Task RemoveInstrumentationBreakpoint(RemoveInstrumentationBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.removeInstrumentationBreakpoint", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Removes breakpoint from XMLHttpRequest. 
        /// </summary>
        public async Task RemoveXHRBreakpoint(RemoveXHRBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.removeXHRBreakpoint", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets breakpoint on particular CSP violations. 
        /// </summary>
        public async Task SetBreakOnCSPViolation(SetBreakOnCSPViolationParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.setBreakOnCSPViolation", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets breakpoint on particular operation with DOM. 
        /// </summary>
        public async Task SetDOMBreakpoint(SetDOMBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.setDOMBreakpoint", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets breakpoint on particular DOM event. 
        /// </summary>
        public async Task SetEventListenerBreakpoint(SetEventListenerBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.setEventListenerBreakpoint", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets breakpoint on particular native event. 
        /// </summary>
        public async Task SetInstrumentationBreakpoint(SetInstrumentationBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.setInstrumentationBreakpoint", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets breakpoint on XMLHttpRequest. 
        /// </summary>
        public async Task SetXHRBreakpoint(SetXHRBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMDebugger.setXHRBreakpoint", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class EventListenerType
        {
            
            /// <summary> `EventListener`'s type. </summary>
            public string type;
            /// <summary> `EventListener`'s useCapture. </summary>
            public bool useCapture;
            /// <summary> `EventListener`'s passive flag. </summary>
            public bool passive;
            /// <summary> `EventListener`'s once flag. </summary>
            public bool once;
            /// <summary> Script id of the handler code. </summary>
            public string scriptId;
            /// <summary> Line number in the script (0-based). </summary>
            public int lineNumber;
            /// <summary> Column number in the script (0-based). </summary>
            public int columnNumber;
            /// <summary> Event handler function value. </summary>
            public Runtime.RemoteObjectType handler;
            /// <summary> Event original handler function value. </summary>
            public Runtime.RemoteObjectType originalHandler;
            /// <summary> Node the listener is added to (if any). </summary>
            public int backendNodeId;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class GetEventListenersParameters
        {
            
            /// <summary> [Require] Identifier of the object to return listeners for. </summary>
            public string objectId;
            /// <summary> [Optional] The maximum depth at which Node children should be retrieved, defaults to 1. Use -1 for theentire subtree or provide an integer larger than 0. </summary>
            public int depth;
            /// <summary> [Optional] Whether or not iframes and shadow roots should be traversed when returning the subtree(default is false). Reports listeners for all contexts if pierce is enabled. </summary>
            public bool pierce;
        }
        public class RemoveDOMBreakpointParameters
        {
            
            /// <summary> [Require] Identifier of the node to remove breakpoint from. </summary>
            public int nodeId;
            /// <summary> [Require] Type of the breakpoint to remove. </summary>
            public string type;
        }
        public class RemoveEventListenerBreakpointParameters
        {
            
            /// <summary> [Require] Event name. </summary>
            public string eventName;
            /// <summary> [Optional] EventTarget interface name. </summary>
            public string targetName;
        }
        public class RemoveInstrumentationBreakpointParameters
        {
            
            /// <summary> [Require] Instrumentation name to stop on. </summary>
            public string eventName;
        }
        public class RemoveXHRBreakpointParameters
        {
            
            /// <summary> [Require] Resource URL substring. </summary>
            public string url;
        }
        public class SetBreakOnCSPViolationParameters
        {
            
            /// <summary> [Require] CSP Violations to stop upon. </summary>
            public object[] violationTypes;
        }
        public class SetDOMBreakpointParameters
        {
            
            /// <summary> [Require] Identifier of the node to set breakpoint on. </summary>
            public int nodeId;
            /// <summary> [Require] Type of the operation to stop upon. </summary>
            public string type;
        }
        public class SetEventListenerBreakpointParameters
        {
            
            /// <summary> [Require] DOM Event name to stop on (any DOM event will do). </summary>
            public string eventName;
            /// <summary> [Optional] EventTarget interface name to stop on. If equal to `"*"` or not provided, will stop on anyEventTarget. </summary>
            public string targetName;
        }
        public class SetInstrumentationBreakpointParameters
        {
            
            /// <summary> [Require] Instrumentation name to stop on. </summary>
            public string eventName;
        }
        public class SetXHRBreakpointParameters
        {
            
            /// <summary> [Require] Resource URL substring. All XHRs having this substring in the URL will get stopped upon. </summary>
            public string url;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetEventListenersReturn
        {
            
            /// <summary> Array of relevant listeners. </summary>
            public object[] listeners;
        }
    }
    
    public class EventBreakpoints : DomainBase
    {
        public EventBreakpoints(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Sets breakpoint on particular native event. 
        /// </summary>
        public async Task SetInstrumentationBreakpoint(SetInstrumentationBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("EventBreakpoints.setInstrumentationBreakpoint", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Removes breakpoint on particular native event. 
        /// </summary>
        public async Task RemoveInstrumentationBreakpoint(RemoveInstrumentationBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("EventBreakpoints.removeInstrumentationBreakpoint", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class SetInstrumentationBreakpointParameters
        {
            
            /// <summary> [Require] Instrumentation name to stop on. </summary>
            public string eventName;
        }
        public class RemoveInstrumentationBreakpointParameters
        {
            
            /// <summary> [Require] Instrumentation name to stop on. </summary>
            public string eventName;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class DOMSnapshot : DomainBase
    {
        public DOMSnapshot(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Disables DOM snapshot agent for the given page. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMSnapshot.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables DOM snapshot agent for the given page. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMSnapshot.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns a document snapshot, including the full DOM tree of the root node (including iframes,template contents, and imported documents) in a flattened array, as well as layout andwhite-listed computed style information for the nodes. Shadow DOM in the returned DOM tree isflattened. 
        /// </summary>
        public async Task<GetSnapshotReturn> GetSnapshot(GetSnapshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMSnapshot.getSnapshot", parameters, sessionId);
            return Convert<GetSnapshotReturn>(___r);
        }
        /// <summary> 
        /// Returns a document snapshot, including the full DOM tree of the root node (including iframes,template contents, and imported documents) in a flattened array, as well as layout andwhite-listed computed style information for the nodes. Shadow DOM in the returned DOM tree isflattened. 
        /// </summary>
        public async Task<CaptureSnapshotReturn> CaptureSnapshot(CaptureSnapshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMSnapshot.captureSnapshot", parameters, sessionId);
            return Convert<CaptureSnapshotReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class DOMNodeType
        {
            
            /// <summary> `Node`'s nodeType. </summary>
            public int nodeType;
            /// <summary> `Node`'s nodeName. </summary>
            public string nodeName;
            /// <summary> `Node`'s nodeValue. </summary>
            public string nodeValue;
            /// <summary> Only set for textarea elements, contains the text value. </summary>
            public string textValue;
            /// <summary> Only set for input elements, contains the input's associated text value. </summary>
            public string inputValue;
            /// <summary> Only set for radio and checkbox input elements, indicates if the element has been checked </summary>
            public bool inputChecked;
            /// <summary> Only set for option elements, indicates if the element has been selected </summary>
            public bool optionSelected;
            /// <summary> `Node`'s id, corresponds to DOM.Node.backendNodeId. </summary>
            public int backendNodeId;
            /// <summary> The indexes of the node's child nodes in the `domNodes` array returned by `getSnapshot`, ifany. </summary>
            public object[] childNodeIndexes;
            /// <summary> Attributes of an `Element` node. </summary>
            public object[] attributes;
            /// <summary> Indexes of pseudo elements associated with this node in the `domNodes` array returned by`getSnapshot`, if any. </summary>
            public object[] pseudoElementIndexes;
            /// <summary> The index of the node's related layout tree node in the `layoutTreeNodes` array returned by`getSnapshot`, if any. </summary>
            public int layoutNodeIndex;
            /// <summary> Document URL that `Document` or `FrameOwner` node points to. </summary>
            public string documentURL;
            /// <summary> Base URL that `Document` or `FrameOwner` node uses for URL completion. </summary>
            public string baseURL;
            /// <summary> Only set for documents, contains the document's content language. </summary>
            public string contentLanguage;
            /// <summary> Only set for documents, contains the document's character set encoding. </summary>
            public string documentEncoding;
            /// <summary> `DocumentType` node's publicId. </summary>
            public string publicId;
            /// <summary> `DocumentType` node's systemId. </summary>
            public string systemId;
            /// <summary> Frame ID for frame owner elements and also for the document node. </summary>
            public string frameId;
            /// <summary> The index of a frame owner element's content document in the `domNodes` array returned by`getSnapshot`, if any. </summary>
            public int contentDocumentIndex;
            /// <summary> Type of a pseudo element node. </summary>
            public string pseudoType;
            /// <summary> Shadow root type. </summary>
            public string shadowRootType;
            /// <summary> Whether this DOM node responds to mouse clicks. This includes nodes that have had clickevent listeners attached via JavaScript as well as anchor tags that naturally navigate whenclicked. </summary>
            public bool isClickable;
            /// <summary> Details of the node's event listeners, if any. </summary>
            public object[] eventListeners;
            /// <summary> The selected url for nodes with a srcset attribute. </summary>
            public string currentSourceURL;
            /// <summary> The url of the script (if any) that generates this node. </summary>
            public string originURL;
            /// <summary> Scroll offsets, set when this node is a Document. </summary>
            public double scrollOffsetX;
            /// <summary>  </summary>
            public double scrollOffsetY;
        }
        public class InlineTextBoxType
        {
            
            /// <summary> The bounding box in document coordinates. Note that scroll offset of the document is ignored. </summary>
            public DOM.RectType boundingBox;
            /// <summary> The starting index in characters, for this post layout textbox substring. Characters thatwould be represented as a surrogate pair in UTF-16 have length 2. </summary>
            public int startCharacterIndex;
            /// <summary> The number of characters in this post layout textbox substring. Characters that would berepresented as a surrogate pair in UTF-16 have length 2. </summary>
            public int numCharacters;
        }
        public class LayoutTreeNodeType
        {
            
            /// <summary> The index of the related DOM node in the `domNodes` array returned by `getSnapshot`. </summary>
            public int domNodeIndex;
            /// <summary> The bounding box in document coordinates. Note that scroll offset of the document is ignored. </summary>
            public DOM.RectType boundingBox;
            /// <summary> Contents of the LayoutText, if any. </summary>
            public string layoutText;
            /// <summary> The post-layout inline text nodes, if any. </summary>
            public object[] inlineTextNodes;
            /// <summary> Index into the `computedStyles` array returned by `getSnapshot`. </summary>
            public int styleIndex;
            /// <summary> Global paint order index, which is determined by the stacking order of the nodes. Nodesthat are painted together will have the same index. Only provided if includePaintOrder ingetSnapshot was true. </summary>
            public int paintOrder;
            /// <summary> Set to true to indicate the element begins a new stacking context. </summary>
            public bool isStackingContext;
        }
        public class ComputedStyleType
        {
            
            /// <summary> Name/value pairs of computed style properties. </summary>
            public object[] properties;
        }
        public class NameValueType
        {
            
            /// <summary> Attribute/property name. </summary>
            public string name;
            /// <summary> Attribute/property value. </summary>
            public string value;
        }
        public class RareStringDataType
        {
            
            /// <summary>  </summary>
            public object[] index;
            /// <summary>  </summary>
            public object[] value;
        }
        public class RareBooleanDataType
        {
            
            /// <summary>  </summary>
            public object[] index;
        }
        public class RareIntegerDataType
        {
            
            /// <summary>  </summary>
            public object[] index;
            /// <summary>  </summary>
            public object[] value;
        }
        public class DocumentSnapshotType
        {
            
            /// <summary> Document URL that `Document` or `FrameOwner` node points to. </summary>
            public int documentURL;
            /// <summary> Document title. </summary>
            public int title;
            /// <summary> Base URL that `Document` or `FrameOwner` node uses for URL completion. </summary>
            public int baseURL;
            /// <summary> Contains the document's content language. </summary>
            public int contentLanguage;
            /// <summary> Contains the document's character set encoding. </summary>
            public int encodingName;
            /// <summary> `DocumentType` node's publicId. </summary>
            public int publicId;
            /// <summary> `DocumentType` node's systemId. </summary>
            public int systemId;
            /// <summary> Frame ID for frame owner elements and also for the document node. </summary>
            public int frameId;
            /// <summary> A table with dom nodes. </summary>
            public DOMSnapshot.NodeTreeSnapshotType nodes;
            /// <summary> The nodes in the layout tree. </summary>
            public DOMSnapshot.LayoutTreeSnapshotType layout;
            /// <summary> The post-layout inline text nodes. </summary>
            public DOMSnapshot.TextBoxSnapshotType textBoxes;
            /// <summary> Horizontal scroll offset. </summary>
            public double scrollOffsetX;
            /// <summary> Vertical scroll offset. </summary>
            public double scrollOffsetY;
            /// <summary> Document content width. </summary>
            public double contentWidth;
            /// <summary> Document content height. </summary>
            public double contentHeight;
        }
        public class NodeTreeSnapshotType
        {
            
            /// <summary> Parent node index. </summary>
            public object[] parentIndex;
            /// <summary> `Node`'s nodeType. </summary>
            public object[] nodeType;
            /// <summary> Type of the shadow root the `Node` is in. String values are equal to the `ShadowRootType` enum. </summary>
            public DOMSnapshot.RareStringDataType shadowRootType;
            /// <summary> `Node`'s nodeName. </summary>
            public object[] nodeName;
            /// <summary> `Node`'s nodeValue. </summary>
            public object[] nodeValue;
            /// <summary> `Node`'s id, corresponds to DOM.Node.backendNodeId. </summary>
            public object[] backendNodeId;
            /// <summary> Attributes of an `Element` node. Flatten name, value pairs. </summary>
            public object[] attributes;
            /// <summary> Only set for textarea elements, contains the text value. </summary>
            public DOMSnapshot.RareStringDataType textValue;
            /// <summary> Only set for input elements, contains the input's associated text value. </summary>
            public DOMSnapshot.RareStringDataType inputValue;
            /// <summary> Only set for radio and checkbox input elements, indicates if the element has been checked </summary>
            public DOMSnapshot.RareBooleanDataType inputChecked;
            /// <summary> Only set for option elements, indicates if the element has been selected </summary>
            public DOMSnapshot.RareBooleanDataType optionSelected;
            /// <summary> The index of the document in the list of the snapshot documents. </summary>
            public DOMSnapshot.RareIntegerDataType contentDocumentIndex;
            /// <summary> Type of a pseudo element node. </summary>
            public DOMSnapshot.RareStringDataType pseudoType;
            /// <summary> Whether this DOM node responds to mouse clicks. This includes nodes that have had clickevent listeners attached via JavaScript as well as anchor tags that naturally navigate whenclicked. </summary>
            public DOMSnapshot.RareBooleanDataType isClickable;
            /// <summary> The selected url for nodes with a srcset attribute. </summary>
            public DOMSnapshot.RareStringDataType currentSourceURL;
            /// <summary> The url of the script (if any) that generates this node. </summary>
            public DOMSnapshot.RareStringDataType originURL;
        }
        public class LayoutTreeSnapshotType
        {
            
            /// <summary> Index of the corresponding node in the `NodeTreeSnapshot` array returned by `captureSnapshot`. </summary>
            public object[] nodeIndex;
            /// <summary> Array of indexes specifying computed style strings, filtered according to the `computedStyles` parameter passed to `captureSnapshot`. </summary>
            public object[] styles;
            /// <summary> The absolute position bounding box. </summary>
            public object[] bounds;
            /// <summary> Contents of the LayoutText, if any. </summary>
            public object[] text;
            /// <summary> Stacking context information. </summary>
            public DOMSnapshot.RareBooleanDataType stackingContexts;
            /// <summary> Global paint order index, which is determined by the stacking order of the nodes. Nodesthat are painted together will have the same index. Only provided if includePaintOrder incaptureSnapshot was true. </summary>
            public object[] paintOrders;
            /// <summary> The offset rect of nodes. Only available when includeDOMRects is set to true </summary>
            public object[] offsetRects;
            /// <summary> The scroll rect of nodes. Only available when includeDOMRects is set to true </summary>
            public object[] scrollRects;
            /// <summary> The client rect of nodes. Only available when includeDOMRects is set to true </summary>
            public object[] clientRects;
            /// <summary> The list of background colors that are blended with colors of overlapping elements. </summary>
            public object[] blendedBackgroundColors;
            /// <summary> The list of computed text opacities. </summary>
            public object[] textColorOpacities;
        }
        public class TextBoxSnapshotType
        {
            
            /// <summary> Index of the layout tree node that owns this box collection. </summary>
            public object[] layoutIndex;
            /// <summary> The absolute position bounding box. </summary>
            public object[] bounds;
            /// <summary> The starting index in characters, for this post layout textbox substring. Characters thatwould be represented as a surrogate pair in UTF-16 have length 2. </summary>
            public object[] start;
            /// <summary> The number of characters in this post layout textbox substring. Characters that would berepresented as a surrogate pair in UTF-16 have length 2. </summary>
            public object[] length;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class GetSnapshotParameters
        {
            
            /// <summary> [Require] Whitelist of computed styles to return. </summary>
            public object[] computedStyleWhitelist;
            /// <summary> [Optional] Whether or not to retrieve details of DOM listeners (default false). </summary>
            public bool includeEventListeners;
            /// <summary> [Optional] Whether to determine and include the paint order index of LayoutTreeNodes (default false). </summary>
            public bool includePaintOrder;
            /// <summary> [Optional] Whether to include UA shadow tree in the snapshot (default false). </summary>
            public bool includeUserAgentShadowTree;
        }
        public class CaptureSnapshotParameters
        {
            
            /// <summary> [Require] Whitelist of computed styles to return. </summary>
            public object[] computedStyles;
            /// <summary> [Optional] Whether to include layout object paint orders into the snapshot. </summary>
            public bool includePaintOrder;
            /// <summary> [Optional] Whether to include DOM rectangles (offsetRects, clientRects, scrollRects) into the snapshot </summary>
            public bool includeDOMRects;
            /// <summary> [Optional] Whether to include blended background colors in the snapshot (default: false).Blended background color is achieved by blending background colors of all elementsthat overlap with the current element. </summary>
            public bool includeBlendedBackgroundColors;
            /// <summary> [Optional] Whether to include text color opacity in the snapshot (default: false).An element might have the opacity property set that affects the text color of the element.The final text color opacity is computed based on the opacity of all overlapping elements. </summary>
            public bool includeTextColorOpacities;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetSnapshotReturn
        {
            
            /// <summary> The nodes in the DOM tree. The DOMNode at index 0 corresponds to the root document. </summary>
            public object[] domNodes;
            /// <summary> The nodes in the layout tree. </summary>
            public object[] layoutTreeNodes;
            /// <summary> Whitelisted ComputedStyle properties for each node in the layout tree. </summary>
            public object[] computedStyles;
        }
        public class CaptureSnapshotReturn
        {
            
            /// <summary> The nodes in the DOM tree. The DOMNode at index 0 corresponds to the root document. </summary>
            public object[] documents;
            /// <summary> Shared string table that all string properties refer to with indexes. </summary>
            public object[] strings;
        }
    }
    
    public class DOMStorage : DomainBase
    {
        public DOMStorage(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnDomStorageItemAdded(Action<OnDomStorageItemAddedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDomStorageItemAddedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOMStorage.domStorageItemAdded" : $"DOMStorage.domStorageItemAdded.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnDomStorageItemRemoved(Action<OnDomStorageItemRemovedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDomStorageItemRemovedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOMStorage.domStorageItemRemoved" : $"DOMStorage.domStorageItemRemoved.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnDomStorageItemUpdated(Action<OnDomStorageItemUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDomStorageItemUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOMStorage.domStorageItemUpdated" : $"DOMStorage.domStorageItemUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnDomStorageItemsCleared(Action<OnDomStorageItemsClearedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDomStorageItemsClearedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "DOMStorage.domStorageItemsCleared" : $"DOMStorage.domStorageItemsCleared.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        ///  
        /// </summary>
        public async Task Clear(ClearParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMStorage.clear", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables storage tracking, prevents storage events from being sent to the client. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMStorage.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables storage tracking, storage events will now be delivered to the client. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMStorage.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetDOMStorageItemsReturn> GetDOMStorageItems(GetDOMStorageItemsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMStorage.getDOMStorageItems", parameters, sessionId);
            return Convert<GetDOMStorageItemsReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task RemoveDOMStorageItem(RemoveDOMStorageItemParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMStorage.removeDOMStorageItem", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetDOMStorageItem(SetDOMStorageItemParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DOMStorage.setDOMStorageItem", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class StorageIdType
        {
            
            /// <summary> Security origin for the storage. </summary>
            public string securityOrigin;
            /// <summary> Whether the storage is local storage (not session storage). </summary>
            public bool isLocalStorage;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnDomStorageItemAddedParameters
        {
            
            /// <summary> [Require]  </summary>
            public DOMStorage.StorageIdType storageId;
            /// <summary> [Require]  </summary>
            public string key;
            /// <summary> [Require]  </summary>
            public string newValue;
        }
        public class OnDomStorageItemRemovedParameters
        {
            
            /// <summary> [Require]  </summary>
            public DOMStorage.StorageIdType storageId;
            /// <summary> [Require]  </summary>
            public string key;
        }
        public class OnDomStorageItemUpdatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public DOMStorage.StorageIdType storageId;
            /// <summary> [Require]  </summary>
            public string key;
            /// <summary> [Require]  </summary>
            public string oldValue;
            /// <summary> [Require]  </summary>
            public string newValue;
        }
        public class OnDomStorageItemsClearedParameters
        {
            
            /// <summary> [Require]  </summary>
            public DOMStorage.StorageIdType storageId;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class ClearParameters
        {
            
            /// <summary> [Require]  </summary>
            public DOMStorage.StorageIdType storageId;
        }
        public class GetDOMStorageItemsParameters
        {
            
            /// <summary> [Require]  </summary>
            public DOMStorage.StorageIdType storageId;
        }
        public class RemoveDOMStorageItemParameters
        {
            
            /// <summary> [Require]  </summary>
            public DOMStorage.StorageIdType storageId;
            /// <summary> [Require]  </summary>
            public string key;
        }
        public class SetDOMStorageItemParameters
        {
            
            /// <summary> [Require]  </summary>
            public DOMStorage.StorageIdType storageId;
            /// <summary> [Require]  </summary>
            public string key;
            /// <summary> [Require]  </summary>
            public string value;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetDOMStorageItemsReturn
        {
            
            /// <summary>  </summary>
            public object[] entries;
        }
    }
    
    public class Database : DomainBase
    {
        public Database(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnAddDatabase(Action<OnAddDatabaseParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAddDatabaseParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Database.addDatabase" : $"Database.addDatabase.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Disables database tracking, prevents database events from being sent to the client. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Database.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables database tracking, database events will now be delivered to the client. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Database.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<ExecuteSQLReturn> ExecuteSQL(ExecuteSQLParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Database.executeSQL", parameters, sessionId);
            return Convert<ExecuteSQLReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetDatabaseTableNamesReturn> GetDatabaseTableNames(GetDatabaseTableNamesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Database.getDatabaseTableNames", parameters, sessionId);
            return Convert<GetDatabaseTableNamesReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class DatabaseType
        {
            
            /// <summary> Database ID. </summary>
            public string id;
            /// <summary> Database domain. </summary>
            public string domain;
            /// <summary> Database name. </summary>
            public string name;
            /// <summary> Database version. </summary>
            public string version;
        }
        public class ErrorType
        {
            
            /// <summary> Error message. </summary>
            public string message;
            /// <summary> Error code. </summary>
            public int code;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnAddDatabaseParameters
        {
            
            /// <summary> [Require]  </summary>
            public Database.DatabaseType database;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class ExecuteSQLParameters
        {
            
            /// <summary> [Require]  </summary>
            public string databaseId;
            /// <summary> [Require]  </summary>
            public string query;
        }
        public class GetDatabaseTableNamesParameters
        {
            
            /// <summary> [Require]  </summary>
            public string databaseId;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class ExecuteSQLReturn
        {
            
            /// <summary>  </summary>
            public object[] columnNames;
            /// <summary>  </summary>
            public object[] values;
            /// <summary>  </summary>
            public Database.ErrorType sqlError;
        }
        public class GetDatabaseTableNamesReturn
        {
            
            /// <summary>  </summary>
            public object[] tableNames;
        }
    }
    
    public class DeviceOrientation : DomainBase
    {
        public DeviceOrientation(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Clears the overridden Device Orientation. 
        /// </summary>
        public async Task ClearDeviceOrientationOverride(string sessionId = default)
        {
            var ___r = await this.chrome.Send("DeviceOrientation.clearDeviceOrientationOverride", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Overrides the Device Orientation. 
        /// </summary>
        public async Task SetDeviceOrientationOverride(SetDeviceOrientationOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("DeviceOrientation.setDeviceOrientationOverride", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class SetDeviceOrientationOverrideParameters
        {
            
            /// <summary> [Require] Mock alpha </summary>
            public double alpha;
            /// <summary> [Require] Mock beta </summary>
            public double beta;
            /// <summary> [Require] Mock gamma </summary>
            public double gamma;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class Emulation : DomainBase
    {
        public Emulation(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Notification sent after the virtual time budget for the current VirtualTimePolicy has run out. </summary>
        /// <returns> remove handler </returns>
        public Action OnVirtualTimeBudgetExpired(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "Emulation.virtualTimeBudgetExpired" : $"Emulation.virtualTimeBudgetExpired.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Tells whether emulation is supported. 
        /// </summary>
        public async Task<CanEmulateReturn> CanEmulate(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.canEmulate", null, sessionId);
            return Convert<CanEmulateReturn>(___r);
        }
        /// <summary> 
        /// Clears the overridden device metrics. 
        /// </summary>
        public async Task ClearDeviceMetricsOverride(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.clearDeviceMetricsOverride", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears the overridden Geolocation Position and Error. 
        /// </summary>
        public async Task ClearGeolocationOverride(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.clearGeolocationOverride", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests that page scale factor is reset to initial values. 
        /// </summary>
        public async Task ResetPageScaleFactor(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.resetPageScaleFactor", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables or disables simulating a focused and active page. 
        /// </summary>
        public async Task SetFocusEmulationEnabled(SetFocusEmulationEnabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setFocusEmulationEnabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Automatically render all web contents using a dark theme. 
        /// </summary>
        public async Task SetAutoDarkModeOverride(SetAutoDarkModeOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setAutoDarkModeOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables CPU throttling to emulate slow CPUs. 
        /// </summary>
        public async Task SetCPUThrottlingRate(SetCPUThrottlingRateParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setCPUThrottlingRate", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets or clears an override of the default background color of the frame. This override is usedif the content does not specify one. 
        /// </summary>
        public async Task SetDefaultBackgroundColorOverride(SetDefaultBackgroundColorOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setDefaultBackgroundColorOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Overrides the values of device screen dimensions (window.screen.width, window.screen.height,window.innerWidth, window.innerHeight, and "device-width"/"device-height"-related CSS mediaquery results). 
        /// </summary>
        public async Task SetDeviceMetricsOverride(SetDeviceMetricsOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setDeviceMetricsOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetScrollbarsHidden(SetScrollbarsHiddenParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setScrollbarsHidden", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetDocumentCookieDisabled(SetDocumentCookieDisabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setDocumentCookieDisabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetEmitTouchEventsForMouse(SetEmitTouchEventsForMouseParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setEmitTouchEventsForMouse", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Emulates the given media type or media feature for CSS media queries. 
        /// </summary>
        public async Task SetEmulatedMedia(SetEmulatedMediaParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setEmulatedMedia", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Emulates the given vision deficiency. 
        /// </summary>
        public async Task SetEmulatedVisionDeficiency(SetEmulatedVisionDeficiencyParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setEmulatedVisionDeficiency", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Overrides the Geolocation Position or Error. Omitting any of the parameters emulates positionunavailable. 
        /// </summary>
        public async Task SetGeolocationOverride(SetGeolocationOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setGeolocationOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Overrides the Idle state. 
        /// </summary>
        public async Task SetIdleOverride(SetIdleOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setIdleOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears Idle state overrides. 
        /// </summary>
        public async Task ClearIdleOverride(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.clearIdleOverride", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Overrides value returned by the javascript navigator object. 
        /// </summary>
        public async Task SetNavigatorOverrides(SetNavigatorOverridesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setNavigatorOverrides", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets a specified page scale factor. 
        /// </summary>
        public async Task SetPageScaleFactor(SetPageScaleFactorParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setPageScaleFactor", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Switches script execution in the page. 
        /// </summary>
        public async Task SetScriptExecutionDisabled(SetScriptExecutionDisabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setScriptExecutionDisabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables touch on platforms which do not support them. 
        /// </summary>
        public async Task SetTouchEmulationEnabled(SetTouchEmulationEnabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setTouchEmulationEnabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Turns on virtual time for all frames (replacing real-time with a synthetic time source) and setsthe current virtual time policy.  Note this supersedes any previous time budget. 
        /// </summary>
        public async Task<SetVirtualTimePolicyReturn> SetVirtualTimePolicy(SetVirtualTimePolicyParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setVirtualTimePolicy", parameters, sessionId);
            return Convert<SetVirtualTimePolicyReturn>(___r);
        }
        /// <summary> 
        /// Overrides default host system locale with the specified one. 
        /// </summary>
        public async Task SetLocaleOverride(SetLocaleOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setLocaleOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Overrides default host system timezone with the specified one. 
        /// </summary>
        public async Task SetTimezoneOverride(SetTimezoneOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setTimezoneOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Resizes the frame/viewport of the page. Note that this does not affect the frame's container(e.g. browser window). Can be used to produce screenshots of the specified size. Not supportedon Android. 
        /// </summary>
        public async Task SetVisibleSize(SetVisibleSizeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setVisibleSize", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetDisabledImageTypes(SetDisabledImageTypesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setDisabledImageTypes", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Allows overriding user agent with the given string. 
        /// </summary>
        public async Task SetUserAgentOverride(SetUserAgentOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Emulation.setUserAgentOverride", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class ScreenOrientationType
        {
            
            /// <summary> Orientation type. </summary>
            public string type;
            /// <summary> Orientation angle. </summary>
            public int angle;
        }
        public class DisplayFeatureType
        {
            
            /// <summary> Orientation of a display feature in relation to screen </summary>
            public string orientation;
            /// <summary> The offset from the screen origin in either the x (for verticalorientation) or y (for horizontal orientation) direction. </summary>
            public int offset;
            /// <summary> A display feature may mask content such that it is not physicallydisplayed - this length along with the offset describes this area.A display feature that only splits content will have a 0 mask_length. </summary>
            public int maskLength;
        }
        public class MediaFeatureType
        {
            
            /// <summary>  </summary>
            public string name;
            /// <summary>  </summary>
            public string value;
        }
        public class UserAgentBrandVersionType
        {
            
            /// <summary>  </summary>
            public string brand;
            /// <summary>  </summary>
            public string version;
        }
        public class UserAgentMetadataType
        {
            
            /// <summary>  </summary>
            public object[] brands;
            /// <summary>  </summary>
            public object[] fullVersionList;
            /// <summary>  </summary>
            public string fullVersion;
            /// <summary>  </summary>
            public string platform;
            /// <summary>  </summary>
            public string platformVersion;
            /// <summary>  </summary>
            public string architecture;
            /// <summary>  </summary>
            public string model;
            /// <summary>  </summary>
            public bool mobile;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class SetFocusEmulationEnabledParameters
        {
            
            /// <summary> [Require] Whether to enable to disable focus emulation. </summary>
            public bool enabled;
        }
        public class SetAutoDarkModeOverrideParameters
        {
            
            /// <summary> [Optional] Whether to enable or disable automatic dark mode.If not specified, any existing override will be cleared. </summary>
            public bool enabled;
        }
        public class SetCPUThrottlingRateParameters
        {
            
            /// <summary> [Require] Throttling rate as a slowdown factor (1 is no throttle, 2 is 2x slowdown, etc). </summary>
            public double rate;
        }
        public class SetDefaultBackgroundColorOverrideParameters
        {
            
            /// <summary> [Optional] RGBA of the default background color. If not specified, any existing override will becleared. </summary>
            public DOM.RGBAType color;
        }
        public class SetDeviceMetricsOverrideParameters
        {
            
            /// <summary> [Require] Overriding width value in pixels (minimum 0, maximum 10000000). 0 disables the override. </summary>
            public int width;
            /// <summary> [Require] Overriding height value in pixels (minimum 0, maximum 10000000). 0 disables the override. </summary>
            public int height;
            /// <summary> [Require] Overriding device scale factor value. 0 disables the override. </summary>
            public double deviceScaleFactor;
            /// <summary> [Require] Whether to emulate mobile device. This includes viewport meta tag, overlay scrollbars, textautosizing and more. </summary>
            public bool mobile;
            /// <summary> [Optional] Scale to apply to resulting view image. </summary>
            public double scale;
            /// <summary> [Optional] Overriding screen width value in pixels (minimum 0, maximum 10000000). </summary>
            public int screenWidth;
            /// <summary> [Optional] Overriding screen height value in pixels (minimum 0, maximum 10000000). </summary>
            public int screenHeight;
            /// <summary> [Optional] Overriding view X position on screen in pixels (minimum 0, maximum 10000000). </summary>
            public int positionX;
            /// <summary> [Optional] Overriding view Y position on screen in pixels (minimum 0, maximum 10000000). </summary>
            public int positionY;
            /// <summary> [Optional] Do not set visible view size, rely upon explicit setVisibleSize call. </summary>
            public bool dontSetVisibleSize;
            /// <summary> [Optional] Screen orientation override. </summary>
            public Emulation.ScreenOrientationType screenOrientation;
            /// <summary> [Optional] If set, the visible area of the page will be overridden to this viewport. This viewportchange is not observed by the page, e.g. viewport-relative elements do not change positions. </summary>
            public Page.ViewportType viewport;
            /// <summary> [Optional] If set, the display feature of a multi-segment screen. If not set, multi-segment supportis turned-off. </summary>
            public Emulation.DisplayFeatureType displayFeature;
        }
        public class SetScrollbarsHiddenParameters
        {
            
            /// <summary> [Require] Whether scrollbars should be always hidden. </summary>
            public bool hidden;
        }
        public class SetDocumentCookieDisabledParameters
        {
            
            /// <summary> [Require] Whether document.coookie API should be disabled. </summary>
            public bool disabled;
        }
        public class SetEmitTouchEventsForMouseParameters
        {
            
            /// <summary> [Require] Whether touch emulation based on mouse input should be enabled. </summary>
            public bool enabled;
            /// <summary> [Optional] Touch/gesture events configuration. Default: current platform. </summary>
            public string configuration;
        }
        public class SetEmulatedMediaParameters
        {
            
            /// <summary> [Optional] Media type to emulate. Empty string disables the override. </summary>
            public string media;
            /// <summary> [Optional] Media features to emulate. </summary>
            public object[] features;
        }
        public class SetEmulatedVisionDeficiencyParameters
        {
            
            /// <summary> [Require] Vision deficiency to emulate. </summary>
            public string type;
        }
        public class SetGeolocationOverrideParameters
        {
            
            /// <summary> [Optional] Mock latitude </summary>
            public double latitude;
            /// <summary> [Optional] Mock longitude </summary>
            public double longitude;
            /// <summary> [Optional] Mock accuracy </summary>
            public double accuracy;
        }
        public class SetIdleOverrideParameters
        {
            
            /// <summary> [Require] Mock isUserActive </summary>
            public bool isUserActive;
            /// <summary> [Require] Mock isScreenUnlocked </summary>
            public bool isScreenUnlocked;
        }
        public class SetNavigatorOverridesParameters
        {
            
            /// <summary> [Require] The platform navigator.platform should return. </summary>
            public string platform;
        }
        public class SetPageScaleFactorParameters
        {
            
            /// <summary> [Require] Page scale factor. </summary>
            public double pageScaleFactor;
        }
        public class SetScriptExecutionDisabledParameters
        {
            
            /// <summary> [Require] Whether script execution should be disabled in the page. </summary>
            public bool value;
        }
        public class SetTouchEmulationEnabledParameters
        {
            
            /// <summary> [Require] Whether the touch event emulation should be enabled. </summary>
            public bool enabled;
            /// <summary> [Optional] Maximum touch points supported. Defaults to one. </summary>
            public int maxTouchPoints;
        }
        public class SetVirtualTimePolicyParameters
        {
            
            /// <summary> [Require]  </summary>
            public string policy;
            /// <summary> [Optional] If set, after this many virtual milliseconds have elapsed virtual time will be paused and avirtualTimeBudgetExpired event is sent. </summary>
            public double budget;
            /// <summary> [Optional] If set this specifies the maximum number of tasks that can be run before virtual is forcedforwards to prevent deadlock. </summary>
            public int maxVirtualTimeTaskStarvationCount;
            /// <summary> [Optional] If set, base::Time::Now will be overridden to initially return this value. </summary>
            public double initialVirtualTime;
        }
        public class SetLocaleOverrideParameters
        {
            
            /// <summary> [Optional] ICU style C locale (e.g. "en_US"). If not specified or empty, disables the override andrestores default host system locale. </summary>
            public string locale;
        }
        public class SetTimezoneOverrideParameters
        {
            
            /// <summary> [Require] The timezone identifier. If empty, disables the override andrestores default host system timezone. </summary>
            public string timezoneId;
        }
        public class SetVisibleSizeParameters
        {
            
            /// <summary> [Require] Frame width (DIP). </summary>
            public int width;
            /// <summary> [Require] Frame height (DIP). </summary>
            public int height;
        }
        public class SetDisabledImageTypesParameters
        {
            
            /// <summary> [Require] Image types to disable. </summary>
            public object[] imageTypes;
        }
        public class SetUserAgentOverrideParameters
        {
            
            /// <summary> [Require] User agent to use. </summary>
            public string userAgent;
            /// <summary> [Optional] Browser langugage to emulate. </summary>
            public string acceptLanguage;
            /// <summary> [Optional] The platform navigator.platform should return. </summary>
            public string platform;
            /// <summary> [Optional] To be sent in Sec-CH-UA-* headers and returned in navigator.userAgentData </summary>
            public Emulation.UserAgentMetadataType userAgentMetadata;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class CanEmulateReturn
        {
            
            /// <summary> True if emulation is supported. </summary>
            public bool result;
        }
        public class SetVirtualTimePolicyReturn
        {
            
            /// <summary> Absolute timestamp at which virtual time was first enabled (up time in milliseconds). </summary>
            public double virtualTimeTicksBase;
        }
    }
    
    public class HeadlessExperimental : DomainBase
    {
        public HeadlessExperimental(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Issued when the target starts or stops needing BeginFrames.Deprecated. Issue beginFrame unconditionally instead and use result frombeginFrame to detect whether the frames were suppressed. </summary>
        /// <returns> remove handler </returns>
        public Action OnNeedsBeginFramesChanged(Action<OnNeedsBeginFramesChangedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnNeedsBeginFramesChangedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "HeadlessExperimental.needsBeginFramesChanged" : $"HeadlessExperimental.needsBeginFramesChanged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Sends a BeginFrame to the target and returns when the frame was completed. Optionally captures ascreenshot from the resulting frame. Requires that the target was created with enabledBeginFrameControl. Designed for use with --run-all-compositor-stages-before-draw, see alsohttps://goo.gl/3zHXhB for more background. 
        /// </summary>
        public async Task<BeginFrameReturn> BeginFrame(BeginFrameParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeadlessExperimental.beginFrame", parameters, sessionId);
            return Convert<BeginFrameReturn>(___r);
        }
        /// <summary> 
        /// Disables headless events for the target. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeadlessExperimental.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables headless events for the target. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeadlessExperimental.enable", null, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class ScreenshotParamsType
        {
            
            /// <summary> Image compression format (defaults to png). </summary>
            public string format;
            /// <summary> Compression quality from range [0..100] (jpeg only). </summary>
            public int quality;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnNeedsBeginFramesChangedParameters
        {
            
            /// <summary> [Require] True if BeginFrames are needed, false otherwise. </summary>
            public bool needsBeginFrames;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class BeginFrameParameters
        {
            
            /// <summary> [Optional] Timestamp of this BeginFrame in Renderer TimeTicks (milliseconds of uptime). If not set,the current time will be used. </summary>
            public double frameTimeTicks;
            /// <summary> [Optional] The interval between BeginFrames that is reported to the compositor, in milliseconds.Defaults to a 60 frames/second interval, i.e. about 16.666 milliseconds. </summary>
            public double interval;
            /// <summary> [Optional] Whether updates should not be committed and drawn onto the display. False by default. Iftrue, only side effects of the BeginFrame will be run, such as layout and animations, butany visual updates may not be visible on the display or in screenshots. </summary>
            public bool noDisplayUpdates;
            /// <summary> [Optional] If set, a screenshot of the frame will be captured and returned in the response. Otherwise,no screenshot will be captured. Note that capturing a screenshot can fail, for example,during renderer initialization. In such a case, no screenshot data will be returned. </summary>
            public HeadlessExperimental.ScreenshotParamsType screenshot;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class BeginFrameReturn
        {
            
            /// <summary> Whether the BeginFrame resulted in damage and, thus, a new frame was committed to thedisplay. Reported for diagnostic uses, may be removed in the future. </summary>
            public bool hasDamage;
            /// <summary> Base64-encoded image data of the screenshot, if one was requested and successfully taken. (Encoded as a base64 string when passed over JSON) </summary>
            public string screenshotData;
        }
    }
    
    public class IO : DomainBase
    {
        public IO(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Close the stream, discard any temporary backing storage. 
        /// </summary>
        public async Task Close(CloseParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IO.close", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Read a chunk of the stream 
        /// </summary>
        public async Task<ReadReturn> Read(ReadParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IO.read", parameters, sessionId);
            return Convert<ReadReturn>(___r);
        }
        /// <summary> 
        /// Return UUID of Blob object specified by a remote object id. 
        /// </summary>
        public async Task<ResolveBlobReturn> ResolveBlob(ResolveBlobParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IO.resolveBlob", parameters, sessionId);
            return Convert<ResolveBlobReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class CloseParameters
        {
            
            /// <summary> [Require] Handle of the stream to close. </summary>
            public string handle;
        }
        public class ReadParameters
        {
            
            /// <summary> [Require] Handle of the stream to read. </summary>
            public string handle;
            /// <summary> [Optional] Seek to the specified offset before reading (if not specificed, proceed with offsetfollowing the last read). Some types of streams may only support sequential reads. </summary>
            public int offset;
            /// <summary> [Optional] Maximum number of bytes to read (left upon the agent discretion if not specified). </summary>
            public int size;
        }
        public class ResolveBlobParameters
        {
            
            /// <summary> [Require] Object id of a Blob object wrapper. </summary>
            public string objectId;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class ReadReturn
        {
            
            /// <summary> Set if the data is base64-encoded </summary>
            public bool base64Encoded;
            /// <summary> Data that were read. </summary>
            public string data;
            /// <summary> Set if the end-of-file condition occurred while reading. </summary>
            public bool eof;
        }
        public class ResolveBlobReturn
        {
            
            /// <summary> UUID of the specified Blob. </summary>
            public string uuid;
        }
    }
    
    public class IndexedDB : DomainBase
    {
        public IndexedDB(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Clears all entries from an object store. 
        /// </summary>
        public async Task ClearObjectStore(ClearObjectStoreParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IndexedDB.clearObjectStore", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Deletes a database. 
        /// </summary>
        public async Task DeleteDatabase(DeleteDatabaseParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IndexedDB.deleteDatabase", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Delete a range of entries from an object store 
        /// </summary>
        public async Task DeleteObjectStoreEntries(DeleteObjectStoreEntriesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IndexedDB.deleteObjectStoreEntries", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables events from backend. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("IndexedDB.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables events from backend. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("IndexedDB.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests data from object store or index. 
        /// </summary>
        public async Task<RequestDataReturn> RequestData(RequestDataParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IndexedDB.requestData", parameters, sessionId);
            return Convert<RequestDataReturn>(___r);
        }
        /// <summary> 
        /// Gets metadata of an object store 
        /// </summary>
        public async Task<GetMetadataReturn> GetMetadata(GetMetadataParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IndexedDB.getMetadata", parameters, sessionId);
            return Convert<GetMetadataReturn>(___r);
        }
        /// <summary> 
        /// Requests database with given name in given frame. 
        /// </summary>
        public async Task<RequestDatabaseReturn> RequestDatabase(RequestDatabaseParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IndexedDB.requestDatabase", parameters, sessionId);
            return Convert<RequestDatabaseReturn>(___r);
        }
        /// <summary> 
        /// Requests database names for given security origin. 
        /// </summary>
        public async Task<RequestDatabaseNamesReturn> RequestDatabaseNames(RequestDatabaseNamesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("IndexedDB.requestDatabaseNames", parameters, sessionId);
            return Convert<RequestDatabaseNamesReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class DatabaseWithObjectStoresType
        {
            
            /// <summary> Database name. </summary>
            public string name;
            /// <summary> Database version (type is not 'integer', as the standardrequires the version number to be 'unsigned long long') </summary>
            public double version;
            /// <summary> Object stores in this database. </summary>
            public object[] objectStores;
        }
        public class ObjectStoreType
        {
            
            /// <summary> Object store name. </summary>
            public string name;
            /// <summary> Object store key path. </summary>
            public IndexedDB.KeyPathType keyPath;
            /// <summary> If true, object store has auto increment flag set. </summary>
            public bool autoIncrement;
            /// <summary> Indexes in this object store. </summary>
            public object[] indexes;
        }
        public class ObjectStoreIndexType
        {
            
            /// <summary> Index name. </summary>
            public string name;
            /// <summary> Index key path. </summary>
            public IndexedDB.KeyPathType keyPath;
            /// <summary> If true, index is unique. </summary>
            public bool unique;
            /// <summary> If true, index allows multiple entries for a key. </summary>
            public bool multiEntry;
        }
        public class KeyType
        {
            
            /// <summary> Key type. </summary>
            public string type;
            /// <summary> Number value. </summary>
            public double number;
            /// <summary> String value. </summary>
            public string @string;
            /// <summary> Date value. </summary>
            public double date;
            /// <summary> Array value. </summary>
            public object[] array;
        }
        public class KeyRangeType
        {
            
            /// <summary> Lower bound. </summary>
            public IndexedDB.KeyType lower;
            /// <summary> Upper bound. </summary>
            public IndexedDB.KeyType upper;
            /// <summary> If true lower bound is open. </summary>
            public bool lowerOpen;
            /// <summary> If true upper bound is open. </summary>
            public bool upperOpen;
        }
        public class DataEntryType
        {
            
            /// <summary> Key object. </summary>
            public Runtime.RemoteObjectType key;
            /// <summary> Primary key object. </summary>
            public Runtime.RemoteObjectType primaryKey;
            /// <summary> Value object. </summary>
            public Runtime.RemoteObjectType value;
        }
        public class KeyPathType
        {
            
            /// <summary> Key path type. </summary>
            public string type;
            /// <summary> String value. </summary>
            public string @string;
            /// <summary> Array value. </summary>
            public object[] array;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class ClearObjectStoreParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string securityOrigin;
            /// <summary> [Require] Database name. </summary>
            public string databaseName;
            /// <summary> [Require] Object store name. </summary>
            public string objectStoreName;
        }
        public class DeleteDatabaseParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string securityOrigin;
            /// <summary> [Require] Database name. </summary>
            public string databaseName;
        }
        public class DeleteObjectStoreEntriesParameters
        {
            
            /// <summary> [Require]  </summary>
            public string securityOrigin;
            /// <summary> [Require]  </summary>
            public string databaseName;
            /// <summary> [Require]  </summary>
            public string objectStoreName;
            /// <summary> [Require] Range of entry keys to delete </summary>
            public IndexedDB.KeyRangeType keyRange;
        }
        public class RequestDataParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string securityOrigin;
            /// <summary> [Require] Database name. </summary>
            public string databaseName;
            /// <summary> [Require] Object store name. </summary>
            public string objectStoreName;
            /// <summary> [Require] Index name, empty string for object store data requests. </summary>
            public string indexName;
            /// <summary> [Require] Number of records to skip. </summary>
            public int skipCount;
            /// <summary> [Require] Number of records to fetch. </summary>
            public int pageSize;
            /// <summary> [Optional] Key range. </summary>
            public IndexedDB.KeyRangeType keyRange;
        }
        public class GetMetadataParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string securityOrigin;
            /// <summary> [Require] Database name. </summary>
            public string databaseName;
            /// <summary> [Require] Object store name. </summary>
            public string objectStoreName;
        }
        public class RequestDatabaseParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string securityOrigin;
            /// <summary> [Require] Database name. </summary>
            public string databaseName;
        }
        public class RequestDatabaseNamesParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string securityOrigin;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class RequestDataReturn
        {
            
            /// <summary> Array of object store data entries. </summary>
            public object[] objectStoreDataEntries;
            /// <summary> If true, there are more entries to fetch in the given range. </summary>
            public bool hasMore;
        }
        public class GetMetadataReturn
        {
            
            /// <summary> the entries count </summary>
            public double entriesCount;
            /// <summary> the current value of key generator, to become the next insertedkey into the object store. Valid if objectStore.autoIncrementis true. </summary>
            public double keyGeneratorValue;
        }
        public class RequestDatabaseReturn
        {
            
            /// <summary> Database with an array of object stores. </summary>
            public IndexedDB.DatabaseWithObjectStoresType databaseWithObjectStores;
        }
        public class RequestDatabaseNamesReturn
        {
            
            /// <summary> Database names for origin. </summary>
            public object[] databaseNames;
        }
    }
    
    public class Input : DomainBase
    {
        public Input(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Emitted only when `Input.setInterceptDrags` is enabled. Use this data with `Input.dispatchDragEvent` torestore normal drag and drop behavior. </summary>
        /// <returns> remove handler </returns>
        public Action OnDragIntercepted(Action<OnDragInterceptedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDragInterceptedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Input.dragIntercepted" : $"Input.dragIntercepted.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Dispatches a drag event into the page. 
        /// </summary>
        public async Task DispatchDragEvent(DispatchDragEventParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.dispatchDragEvent", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Dispatches a key event to the page. 
        /// </summary>
        public async Task DispatchKeyEvent(DispatchKeyEventParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.dispatchKeyEvent", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// This method emulates inserting text that doesn't come from a key press,for example an emoji keyboard or an IME. 
        /// </summary>
        public async Task InsertText(InsertTextParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.insertText", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// This method sets the current candidate text for ime.Use imeCommitComposition to commit the final text.Use imeSetComposition with empty string as text to cancel composition. 
        /// </summary>
        public async Task ImeSetComposition(ImeSetCompositionParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.imeSetComposition", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Dispatches a mouse event to the page. 
        /// </summary>
        public async Task DispatchMouseEvent(DispatchMouseEventParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.dispatchMouseEvent", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Dispatches a touch event to the page. 
        /// </summary>
        public async Task DispatchTouchEvent(DispatchTouchEventParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.dispatchTouchEvent", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Emulates touch event from the mouse event parameters. 
        /// </summary>
        public async Task EmulateTouchFromMouseEvent(EmulateTouchFromMouseEventParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.emulateTouchFromMouseEvent", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Ignores input events (useful while auditing page). 
        /// </summary>
        public async Task SetIgnoreInputEvents(SetIgnoreInputEventsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.setIgnoreInputEvents", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Prevents default drag and drop behavior and instead emits `Input.dragIntercepted` events.Drag and drop behavior can be directly controlled via `Input.dispatchDragEvent`. 
        /// </summary>
        public async Task SetInterceptDrags(SetInterceptDragsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.setInterceptDrags", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Synthesizes a pinch gesture over a time period by issuing appropriate touch events. 
        /// </summary>
        public async Task SynthesizePinchGesture(SynthesizePinchGestureParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.synthesizePinchGesture", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Synthesizes a scroll gesture over a time period by issuing appropriate touch events. 
        /// </summary>
        public async Task SynthesizeScrollGesture(SynthesizeScrollGestureParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.synthesizeScrollGesture", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Synthesizes a tap gesture over a time period by issuing appropriate touch events. 
        /// </summary>
        public async Task SynthesizeTapGesture(SynthesizeTapGestureParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Input.synthesizeTapGesture", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class TouchPointType
        {
            
            /// <summary> X coordinate of the event relative to the main frame's viewport in CSS pixels. </summary>
            public double x;
            /// <summary> Y coordinate of the event relative to the main frame's viewport in CSS pixels. 0 refers tothe top of the viewport and Y increases as it proceeds towards the bottom of the viewport. </summary>
            public double y;
            /// <summary> X radius of the touch area (default: 1.0). </summary>
            public double radiusX;
            /// <summary> Y radius of the touch area (default: 1.0). </summary>
            public double radiusY;
            /// <summary> Rotation angle (default: 0.0). </summary>
            public double rotationAngle;
            /// <summary> Force (default: 1.0). </summary>
            public double force;
            /// <summary> The normalized tangential pressure, which has a range of [-1,1] (default: 0). </summary>
            public double tangentialPressure;
            /// <summary> The plane angle between the Y-Z plane and the plane containing both the stylus axis and the Y axis, in degrees of the range [-90,90], a positive tiltX is to the right (default: 0) </summary>
            public int tiltX;
            /// <summary> The plane angle between the X-Z plane and the plane containing both the stylus axis and the X axis, in degrees of the range [-90,90], a positive tiltY is towards the user (default: 0). </summary>
            public int tiltY;
            /// <summary> The clockwise rotation of a pen stylus around its own major axis, in degrees in the range [0,359] (default: 0). </summary>
            public int twist;
            /// <summary> Identifier used to track touch sources between events, must be unique within an event. </summary>
            public double id;
        }
        public class DragDataItemType
        {
            
            /// <summary> Mime type of the dragged data. </summary>
            public string mimeType;
            /// <summary> Depending of the value of `mimeType`, it contains the dragged link,text, HTML markup or any other data. </summary>
            public string data;
            /// <summary> Title associated with a link. Only valid when `mimeType` == "text/uri-list". </summary>
            public string title;
            /// <summary> Stores the base URL for the contained markup. Only valid when `mimeType`== "text/html". </summary>
            public string baseURL;
        }
        public class DragDataType
        {
            
            /// <summary>  </summary>
            public object[] items;
            /// <summary> List of filenames that should be included when dropping </summary>
            public object[] files;
            /// <summary> Bit field representing allowed drag operations. Copy = 1, Link = 2, Move = 16 </summary>
            public int dragOperationsMask;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnDragInterceptedParameters
        {
            
            /// <summary> [Require]  </summary>
            public Input.DragDataType data;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class DispatchDragEventParameters
        {
            
            /// <summary> [Require] Type of the drag event. </summary>
            public string type;
            /// <summary> [Require] X coordinate of the event relative to the main frame's viewport in CSS pixels. </summary>
            public double x;
            /// <summary> [Require] Y coordinate of the event relative to the main frame's viewport in CSS pixels. 0 refers tothe top of the viewport and Y increases as it proceeds towards the bottom of the viewport. </summary>
            public double y;
            /// <summary> [Require]  </summary>
            public Input.DragDataType data;
            /// <summary> [Optional] Bit field representing pressed modifier keys. Alt=1, Ctrl=2, Meta/Command=4, Shift=8(default: 0). </summary>
            public int modifiers;
        }
        public class DispatchKeyEventParameters
        {
            
            /// <summary> [Require] Type of the key event. </summary>
            public string type;
            /// <summary> [Optional] Bit field representing pressed modifier keys. Alt=1, Ctrl=2, Meta/Command=4, Shift=8(default: 0). </summary>
            public int modifiers;
            /// <summary> [Optional] Time at which the event occurred. </summary>
            public double timestamp;
            /// <summary> [Optional] Text as generated by processing a virtual key code with a keyboard layout. Not needed forfor `keyUp` and `rawKeyDown` events (default: "") </summary>
            public string text;
            /// <summary> [Optional] Text that would have been generated by the keyboard if no modifiers were pressed (except forshift). Useful for shortcut (accelerator) key handling (default: ""). </summary>
            public string unmodifiedText;
            /// <summary> [Optional] Unique key identifier (e.g., 'U+0041') (default: ""). </summary>
            public string keyIdentifier;
            /// <summary> [Optional] Unique DOM defined string value for each physical key (e.g., 'KeyA') (default: ""). </summary>
            public string code;
            /// <summary> [Optional] Unique DOM defined string value describing the meaning of the key in the context of activemodifiers, keyboard layout, etc (e.g., 'AltGr') (default: ""). </summary>
            public string key;
            /// <summary> [Optional] Windows virtual key code (default: 0). </summary>
            public int windowsVirtualKeyCode;
            /// <summary> [Optional] Native virtual key code (default: 0). </summary>
            public int nativeVirtualKeyCode;
            /// <summary> [Optional] Whether the event was generated from auto repeat (default: false). </summary>
            public bool autoRepeat;
            /// <summary> [Optional] Whether the event was generated from the keypad (default: false). </summary>
            public bool isKeypad;
            /// <summary> [Optional] Whether the event was a system key event (default: false). </summary>
            public bool isSystemKey;
            /// <summary> [Optional] Whether the event was from the left or right side of the keyboard. 1=Left, 2=Right (default:0). </summary>
            public int location;
            /// <summary> [Optional] Editing commands to send with the key event (e.g., 'selectAll') (default: []).These are related to but not equal the command names used in `document.execCommand` and NSStandardKeyBindingResponding.See https://source.chromium.org/chromium/chromium/src/+/main:third_party/blink/renderer/core/editing/commands/editor_command_names.h for valid command names. </summary>
            public object[] commands;
        }
        public class InsertTextParameters
        {
            
            /// <summary> [Require] The text to insert. </summary>
            public string text;
        }
        public class ImeSetCompositionParameters
        {
            
            /// <summary> [Require] The text to insert </summary>
            public string text;
            /// <summary> [Require] selection start </summary>
            public int selectionStart;
            /// <summary> [Require] selection end </summary>
            public int selectionEnd;
            /// <summary> [Optional] replacement start </summary>
            public int replacementStart;
            /// <summary> [Optional] replacement end </summary>
            public int replacementEnd;
        }
        public class DispatchMouseEventParameters
        {
            
            /// <summary> [Require] Type of the mouse event. </summary>
            public string type;
            /// <summary> [Require] X coordinate of the event relative to the main frame's viewport in CSS pixels. </summary>
            public double x;
            /// <summary> [Require] Y coordinate of the event relative to the main frame's viewport in CSS pixels. 0 refers tothe top of the viewport and Y increases as it proceeds towards the bottom of the viewport. </summary>
            public double y;
            /// <summary> [Optional] Bit field representing pressed modifier keys. Alt=1, Ctrl=2, Meta/Command=4, Shift=8(default: 0). </summary>
            public int modifiers;
            /// <summary> [Optional] Time at which the event occurred. </summary>
            public double timestamp;
            /// <summary> [Optional] Mouse button (default: "none"). </summary>
            public string button;
            /// <summary> [Optional] A number indicating which buttons are pressed on the mouse when a mouse event is triggered.Left=1, Right=2, Middle=4, Back=8, Forward=16, None=0. </summary>
            public int buttons;
            /// <summary> [Optional] Number of times the mouse button was clicked (default: 0). </summary>
            public int clickCount;
            /// <summary> [Optional] The normalized pressure, which has a range of [0,1] (default: 0). </summary>
            public double force;
            /// <summary> [Optional] The normalized tangential pressure, which has a range of [-1,1] (default: 0). </summary>
            public double tangentialPressure;
            /// <summary> [Optional] The plane angle between the Y-Z plane and the plane containing both the stylus axis and the Y axis, in degrees of the range [-90,90], a positive tiltX is to the right (default: 0). </summary>
            public int tiltX;
            /// <summary> [Optional] The plane angle between the X-Z plane and the plane containing both the stylus axis and the X axis, in degrees of the range [-90,90], a positive tiltY is towards the user (default: 0). </summary>
            public int tiltY;
            /// <summary> [Optional] The clockwise rotation of a pen stylus around its own major axis, in degrees in the range [0,359] (default: 0). </summary>
            public int twist;
            /// <summary> [Optional] X delta in CSS pixels for mouse wheel event (default: 0). </summary>
            public double deltaX;
            /// <summary> [Optional] Y delta in CSS pixels for mouse wheel event (default: 0). </summary>
            public double deltaY;
            /// <summary> [Optional] Pointer type (default: "mouse"). </summary>
            public string pointerType;
        }
        public class DispatchTouchEventParameters
        {
            
            /// <summary> [Require] Type of the touch event. TouchEnd and TouchCancel must not contain any touch points, whileTouchStart and TouchMove must contains at least one. </summary>
            public string type;
            /// <summary> [Require] Active touch points on the touch device. One event per any changed point (compared toprevious touch event in a sequence) is generated, emulating pressing/moving/releasing pointsone by one. </summary>
            public object[] touchPoints;
            /// <summary> [Optional] Bit field representing pressed modifier keys. Alt=1, Ctrl=2, Meta/Command=4, Shift=8(default: 0). </summary>
            public int modifiers;
            /// <summary> [Optional] Time at which the event occurred. </summary>
            public double timestamp;
        }
        public class EmulateTouchFromMouseEventParameters
        {
            
            /// <summary> [Require] Type of the mouse event. </summary>
            public string type;
            /// <summary> [Require] X coordinate of the mouse pointer in DIP. </summary>
            public int x;
            /// <summary> [Require] Y coordinate of the mouse pointer in DIP. </summary>
            public int y;
            /// <summary> [Require] Mouse button. Only "none", "left", "right" are supported. </summary>
            public string button;
            /// <summary> [Optional] Time at which the event occurred (default: current time). </summary>
            public double timestamp;
            /// <summary> [Optional] X delta in DIP for mouse wheel event (default: 0). </summary>
            public double deltaX;
            /// <summary> [Optional] Y delta in DIP for mouse wheel event (default: 0). </summary>
            public double deltaY;
            /// <summary> [Optional] Bit field representing pressed modifier keys. Alt=1, Ctrl=2, Meta/Command=4, Shift=8(default: 0). </summary>
            public int modifiers;
            /// <summary> [Optional] Number of times the mouse button was clicked (default: 0). </summary>
            public int clickCount;
        }
        public class SetIgnoreInputEventsParameters
        {
            
            /// <summary> [Require] Ignores input events processing when set to true. </summary>
            public bool ignore;
        }
        public class SetInterceptDragsParameters
        {
            
            /// <summary> [Require]  </summary>
            public bool enabled;
        }
        public class SynthesizePinchGestureParameters
        {
            
            /// <summary> [Require] X coordinate of the start of the gesture in CSS pixels. </summary>
            public double x;
            /// <summary> [Require] Y coordinate of the start of the gesture in CSS pixels. </summary>
            public double y;
            /// <summary> [Require] Relative scale factor after zooming (>1.0 zooms in, <1.0 zooms out). </summary>
            public double scaleFactor;
            /// <summary> [Optional] Relative pointer speed in pixels per second (default: 800). </summary>
            public int relativeSpeed;
            /// <summary> [Optional] Which type of input events to be generated (default: 'default', which queries the platformfor the preferred input type). </summary>
            public string gestureSourceType;
        }
        public class SynthesizeScrollGestureParameters
        {
            
            /// <summary> [Require] X coordinate of the start of the gesture in CSS pixels. </summary>
            public double x;
            /// <summary> [Require] Y coordinate of the start of the gesture in CSS pixels. </summary>
            public double y;
            /// <summary> [Optional] The distance to scroll along the X axis (positive to scroll left). </summary>
            public double xDistance;
            /// <summary> [Optional] The distance to scroll along the Y axis (positive to scroll up). </summary>
            public double yDistance;
            /// <summary> [Optional] The number of additional pixels to scroll back along the X axis, in addition to the givendistance. </summary>
            public double xOverscroll;
            /// <summary> [Optional] The number of additional pixels to scroll back along the Y axis, in addition to the givendistance. </summary>
            public double yOverscroll;
            /// <summary> [Optional] Prevent fling (default: true). </summary>
            public bool preventFling;
            /// <summary> [Optional] Swipe speed in pixels per second (default: 800). </summary>
            public int speed;
            /// <summary> [Optional] Which type of input events to be generated (default: 'default', which queries the platformfor the preferred input type). </summary>
            public string gestureSourceType;
            /// <summary> [Optional] The number of times to repeat the gesture (default: 0). </summary>
            public int repeatCount;
            /// <summary> [Optional] The number of milliseconds delay between each repeat. (default: 250). </summary>
            public int repeatDelayMs;
            /// <summary> [Optional] The name of the interaction markers to generate, if not empty (default: ""). </summary>
            public string interactionMarkerName;
        }
        public class SynthesizeTapGestureParameters
        {
            
            /// <summary> [Require] X coordinate of the start of the gesture in CSS pixels. </summary>
            public double x;
            /// <summary> [Require] Y coordinate of the start of the gesture in CSS pixels. </summary>
            public double y;
            /// <summary> [Optional] Duration between touchdown and touchup events in ms (default: 50). </summary>
            public int duration;
            /// <summary> [Optional] Number of times to perform the tap (e.g. 2 for double tap, default: 1). </summary>
            public int tapCount;
            /// <summary> [Optional] Which type of input events to be generated (default: 'default', which queries the platformfor the preferred input type). </summary>
            public string gestureSourceType;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class Inspector : DomainBase
    {
        public Inspector(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Fired when remote debugging connection is about to be terminated. Contains detach reason. </summary>
        /// <returns> remove handler </returns>
        public Action OnDetached(Action<OnDetachedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDetachedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Inspector.detached" : $"Inspector.detached.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when debugging target has crashed </summary>
        /// <returns> remove handler </returns>
        public Action OnTargetCrashed(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "Inspector.targetCrashed" : $"Inspector.targetCrashed.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        
        /// <summary> Fired when debugging target has reloaded after crash </summary>
        /// <returns> remove handler </returns>
        public Action OnTargetReloadedAfterCrash(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "Inspector.targetReloadedAfterCrash" : $"Inspector.targetReloadedAfterCrash.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Disables inspector domain notifications. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Inspector.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables inspector domain notifications. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Inspector.enable", null, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnDetachedParameters
        {
            
            /// <summary> [Require] The reason why connection has been terminated. </summary>
            public string reason;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class LayerTree : DomainBase
    {
        public LayerTree(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnLayerPainted(Action<OnLayerPaintedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnLayerPaintedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "LayerTree.layerPainted" : $"LayerTree.layerPainted.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnLayerTreeDidChange(Action<OnLayerTreeDidChangeParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnLayerTreeDidChangeParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "LayerTree.layerTreeDidChange" : $"LayerTree.layerTreeDidChange.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Provides the reasons why the given layer was composited. 
        /// </summary>
        public async Task<CompositingReasonsReturn> CompositingReasons(CompositingReasonsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("LayerTree.compositingReasons", parameters, sessionId);
            return Convert<CompositingReasonsReturn>(___r);
        }
        /// <summary> 
        /// Disables compositing tree inspection. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("LayerTree.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables compositing tree inspection. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("LayerTree.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns the snapshot identifier. 
        /// </summary>
        public async Task<LoadSnapshotReturn> LoadSnapshot(LoadSnapshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("LayerTree.loadSnapshot", parameters, sessionId);
            return Convert<LoadSnapshotReturn>(___r);
        }
        /// <summary> 
        /// Returns the layer snapshot identifier. 
        /// </summary>
        public async Task<MakeSnapshotReturn> MakeSnapshot(MakeSnapshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("LayerTree.makeSnapshot", parameters, sessionId);
            return Convert<MakeSnapshotReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<ProfileSnapshotReturn> ProfileSnapshot(ProfileSnapshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("LayerTree.profileSnapshot", parameters, sessionId);
            return Convert<ProfileSnapshotReturn>(___r);
        }
        /// <summary> 
        /// Releases layer snapshot captured by the back-end. 
        /// </summary>
        public async Task ReleaseSnapshot(ReleaseSnapshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("LayerTree.releaseSnapshot", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Replays the layer snapshot and returns the resulting bitmap. 
        /// </summary>
        public async Task<ReplaySnapshotReturn> ReplaySnapshot(ReplaySnapshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("LayerTree.replaySnapshot", parameters, sessionId);
            return Convert<ReplaySnapshotReturn>(___r);
        }
        /// <summary> 
        /// Replays the layer snapshot and returns canvas log. 
        /// </summary>
        public async Task<SnapshotCommandLogReturn> SnapshotCommandLog(SnapshotCommandLogParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("LayerTree.snapshotCommandLog", parameters, sessionId);
            return Convert<SnapshotCommandLogReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class ScrollRectType
        {
            
            /// <summary> Rectangle itself. </summary>
            public DOM.RectType rect;
            /// <summary> Reason for rectangle to force scrolling on the main thread </summary>
            public string type;
        }
        public class StickyPositionConstraintType
        {
            
            /// <summary> Layout rectangle of the sticky element before being shifted </summary>
            public DOM.RectType stickyBoxRect;
            /// <summary> Layout rectangle of the containing block of the sticky element </summary>
            public DOM.RectType containingBlockRect;
            /// <summary> The nearest sticky layer that shifts the sticky box </summary>
            public string nearestLayerShiftingStickyBox;
            /// <summary> The nearest sticky layer that shifts the containing block </summary>
            public string nearestLayerShiftingContainingBlock;
        }
        public class PictureTileType
        {
            
            /// <summary> Offset from owning layer left boundary </summary>
            public double x;
            /// <summary> Offset from owning layer top boundary </summary>
            public double y;
            /// <summary> Base64-encoded snapshot data. (Encoded as a base64 string when passed over JSON) </summary>
            public string picture;
        }
        public class LayerType
        {
            
            /// <summary> The unique id for this layer. </summary>
            public string layerId;
            /// <summary> The id of parent (not present for root). </summary>
            public string parentLayerId;
            /// <summary> The backend id for the node associated with this layer. </summary>
            public int backendNodeId;
            /// <summary> Offset from parent layer, X coordinate. </summary>
            public double offsetX;
            /// <summary> Offset from parent layer, Y coordinate. </summary>
            public double offsetY;
            /// <summary> Layer width. </summary>
            public double width;
            /// <summary> Layer height. </summary>
            public double height;
            /// <summary> Transformation matrix for layer, default is identity matrix </summary>
            public object[] transform;
            /// <summary> Transform anchor point X, absent if no transform specified </summary>
            public double anchorX;
            /// <summary> Transform anchor point Y, absent if no transform specified </summary>
            public double anchorY;
            /// <summary> Transform anchor point Z, absent if no transform specified </summary>
            public double anchorZ;
            /// <summary> Indicates how many time this layer has painted. </summary>
            public int paintCount;
            /// <summary> Indicates whether this layer hosts any content, rather than being used fortransform/scrolling purposes only. </summary>
            public bool drawsContent;
            /// <summary> Set if layer is not visible. </summary>
            public bool invisible;
            /// <summary> Rectangles scrolling on main thread only. </summary>
            public object[] scrollRects;
            /// <summary> Sticky position constraint information </summary>
            public LayerTree.StickyPositionConstraintType stickyPositionConstraint;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnLayerPaintedParameters
        {
            
            /// <summary> [Require] The id of the painted layer. </summary>
            public string layerId;
            /// <summary> [Require] Clip rectangle. </summary>
            public DOM.RectType clip;
        }
        public class OnLayerTreeDidChangeParameters
        {
            
            /// <summary> [Optional] Layer tree, absent if not in the comspositing mode. </summary>
            public object[] layers;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class CompositingReasonsParameters
        {
            
            /// <summary> [Require] The id of the layer for which we want to get the reasons it was composited. </summary>
            public string layerId;
        }
        public class LoadSnapshotParameters
        {
            
            /// <summary> [Require] An array of tiles composing the snapshot. </summary>
            public object[] tiles;
        }
        public class MakeSnapshotParameters
        {
            
            /// <summary> [Require] The id of the layer. </summary>
            public string layerId;
        }
        public class ProfileSnapshotParameters
        {
            
            /// <summary> [Require] The id of the layer snapshot. </summary>
            public string snapshotId;
            /// <summary> [Optional] The maximum number of times to replay the snapshot (1, if not specified). </summary>
            public int minRepeatCount;
            /// <summary> [Optional] The minimum duration (in seconds) to replay the snapshot. </summary>
            public double minDuration;
            /// <summary> [Optional] The clip rectangle to apply when replaying the snapshot. </summary>
            public DOM.RectType clipRect;
        }
        public class ReleaseSnapshotParameters
        {
            
            /// <summary> [Require] The id of the layer snapshot. </summary>
            public string snapshotId;
        }
        public class ReplaySnapshotParameters
        {
            
            /// <summary> [Require] The id of the layer snapshot. </summary>
            public string snapshotId;
            /// <summary> [Optional] The first step to replay from (replay from the very start if not specified). </summary>
            public int fromStep;
            /// <summary> [Optional] The last step to replay to (replay till the end if not specified). </summary>
            public int toStep;
            /// <summary> [Optional] The scale to apply while replaying (defaults to 1). </summary>
            public double scale;
        }
        public class SnapshotCommandLogParameters
        {
            
            /// <summary> [Require] The id of the layer snapshot. </summary>
            public string snapshotId;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class CompositingReasonsReturn
        {
            
            /// <summary> A list of strings specifying reasons for the given layer to become composited. </summary>
            public object[] compositingReasons;
            /// <summary> A list of strings specifying reason IDs for the given layer to become composited. </summary>
            public object[] compositingReasonIds;
        }
        public class LoadSnapshotReturn
        {
            
            /// <summary> The id of the snapshot. </summary>
            public string snapshotId;
        }
        public class MakeSnapshotReturn
        {
            
            /// <summary> The id of the layer snapshot. </summary>
            public string snapshotId;
        }
        public class ProfileSnapshotReturn
        {
            
            /// <summary> The array of paint profiles, one per run. </summary>
            public object[] timings;
        }
        public class ReplaySnapshotReturn
        {
            
            /// <summary> A data: URL for resulting image. </summary>
            public string dataURL;
        }
        public class SnapshotCommandLogReturn
        {
            
            /// <summary> The array of canvas function calls. </summary>
            public object[] commandLog;
        }
    }
    
    public class Log : DomainBase
    {
        public Log(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Issued when new message was logged. </summary>
        /// <returns> remove handler </returns>
        public Action OnEntryAdded(Action<OnEntryAddedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnEntryAddedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Log.entryAdded" : $"Log.entryAdded.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Clears the log. 
        /// </summary>
        public async Task Clear(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Log.clear", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables log domain, prevents further log entries from being reported to the client. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Log.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables log domain, sends the entries collected so far to the client by means of the`entryAdded` notification. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Log.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// start violation reporting. 
        /// </summary>
        public async Task StartViolationsReport(StartViolationsReportParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Log.startViolationsReport", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Stop violation reporting. 
        /// </summary>
        public async Task StopViolationsReport(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Log.stopViolationsReport", null, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class LogEntryType
        {
            
            /// <summary> Log entry source. </summary>
            public string source;
            /// <summary> Log entry severity. </summary>
            public string level;
            /// <summary> Logged text. </summary>
            public string text;
            /// <summary>  </summary>
            public string category;
            /// <summary> Timestamp when this entry was added. </summary>
            public double timestamp;
            /// <summary> URL of the resource if known. </summary>
            public string url;
            /// <summary> Line number in the resource. </summary>
            public int lineNumber;
            /// <summary> JavaScript stack trace. </summary>
            public Runtime.StackTraceType stackTrace;
            /// <summary> Identifier of the network request associated with this entry. </summary>
            public string networkRequestId;
            /// <summary> Identifier of the worker associated with this entry. </summary>
            public string workerId;
            /// <summary> Call arguments. </summary>
            public object[] args;
        }
        public class ViolationSettingType
        {
            
            /// <summary> Violation type. </summary>
            public string name;
            /// <summary> Time threshold to trigger upon. </summary>
            public double threshold;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnEntryAddedParameters
        {
            
            /// <summary> [Require] The entry. </summary>
            public Log.LogEntryType entry;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class StartViolationsReportParameters
        {
            
            /// <summary> [Require] Configuration for violations. </summary>
            public object[] config;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class Memory : DomainBase
    {
        public Memory(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetDOMCountersReturn> GetDOMCounters(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.getDOMCounters", null, sessionId);
            return Convert<GetDOMCountersReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task PrepareForLeakDetection(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.prepareForLeakDetection", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Simulate OomIntervention by purging V8 memory. 
        /// </summary>
        public async Task ForciblyPurgeJavaScriptMemory(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.forciblyPurgeJavaScriptMemory", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enable/disable suppressing memory pressure notifications in all processes. 
        /// </summary>
        public async Task SetPressureNotificationsSuppressed(SetPressureNotificationsSuppressedParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.setPressureNotificationsSuppressed", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Simulate a memory pressure notification in all processes. 
        /// </summary>
        public async Task SimulatePressureNotification(SimulatePressureNotificationParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.simulatePressureNotification", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Start collecting native memory profile. 
        /// </summary>
        public async Task StartSampling(StartSamplingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.startSampling", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Stop collecting native memory profile. 
        /// </summary>
        public async Task StopSampling(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.stopSampling", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Retrieve native memory allocations profilecollected since renderer process startup. 
        /// </summary>
        public async Task<GetAllTimeSamplingProfileReturn> GetAllTimeSamplingProfile(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.getAllTimeSamplingProfile", null, sessionId);
            return Convert<GetAllTimeSamplingProfileReturn>(___r);
        }
        /// <summary> 
        /// Retrieve native memory allocations profilecollected since browser process startup. 
        /// </summary>
        public async Task<GetBrowserSamplingProfileReturn> GetBrowserSamplingProfile(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.getBrowserSamplingProfile", null, sessionId);
            return Convert<GetBrowserSamplingProfileReturn>(___r);
        }
        /// <summary> 
        /// Retrieve native memory allocations profile collected since last`startSampling` call. 
        /// </summary>
        public async Task<GetSamplingProfileReturn> GetSamplingProfile(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Memory.getSamplingProfile", null, sessionId);
            return Convert<GetSamplingProfileReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class SamplingProfileNodeType
        {
            
            /// <summary> Size of the sampled allocation. </summary>
            public double size;
            /// <summary> Total bytes attributed to this sample. </summary>
            public double total;
            /// <summary> Execution stack at the point of allocation. </summary>
            public object[] stack;
        }
        public class SamplingProfileType
        {
            
            /// <summary>  </summary>
            public object[] samples;
            /// <summary>  </summary>
            public object[] modules;
        }
        public class ModuleType
        {
            
            /// <summary> Name of the module. </summary>
            public string name;
            /// <summary> UUID of the module. </summary>
            public string uuid;
            /// <summary> Base address where the module is loaded into memory. Encoded as a decimalor hexadecimal (0x prefixed) string. </summary>
            public string baseAddress;
            /// <summary> Size of the module in bytes. </summary>
            public double size;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class SetPressureNotificationsSuppressedParameters
        {
            
            /// <summary> [Require] If true, memory pressure notifications will be suppressed. </summary>
            public bool suppressed;
        }
        public class SimulatePressureNotificationParameters
        {
            
            /// <summary> [Require] Memory pressure level of the notification. </summary>
            public string level;
        }
        public class StartSamplingParameters
        {
            
            /// <summary> [Optional] Average number of bytes between samples. </summary>
            public int samplingInterval;
            /// <summary> [Optional] Do not randomize intervals between samples. </summary>
            public bool suppressRandomness;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetDOMCountersReturn
        {
            
            /// <summary>  </summary>
            public int documents;
            /// <summary>  </summary>
            public int nodes;
            /// <summary>  </summary>
            public int jsEventListeners;
        }
        public class GetAllTimeSamplingProfileReturn
        {
            
            /// <summary>  </summary>
            public Memory.SamplingProfileType profile;
        }
        public class GetBrowserSamplingProfileReturn
        {
            
            /// <summary>  </summary>
            public Memory.SamplingProfileType profile;
        }
        public class GetSamplingProfileReturn
        {
            
            /// <summary>  </summary>
            public Memory.SamplingProfileType profile;
        }
    }
    
    public class Network : DomainBase
    {
        public Network(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Fired when data chunk was received over the network. </summary>
        /// <returns> remove handler </returns>
        public Action OnDataReceived(Action<OnDataReceivedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDataReceivedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.dataReceived" : $"Network.dataReceived.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when EventSource message is received. </summary>
        /// <returns> remove handler </returns>
        public Action OnEventSourceMessageReceived(Action<OnEventSourceMessageReceivedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnEventSourceMessageReceivedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.eventSourceMessageReceived" : $"Network.eventSourceMessageReceived.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when HTTP request has failed to load. </summary>
        /// <returns> remove handler </returns>
        public Action OnLoadingFailed(Action<OnLoadingFailedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnLoadingFailedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.loadingFailed" : $"Network.loadingFailed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when HTTP request has finished loading. </summary>
        /// <returns> remove handler </returns>
        public Action OnLoadingFinished(Action<OnLoadingFinishedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnLoadingFinishedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.loadingFinished" : $"Network.loadingFinished.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Details of an intercepted HTTP request, which must be either allowed, blocked, modified ormocked.Deprecated, use Fetch.requestPaused instead. </summary>
        /// <returns> remove handler </returns>
        public Action OnRequestIntercepted(Action<OnRequestInterceptedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnRequestInterceptedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.requestIntercepted" : $"Network.requestIntercepted.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired if request ended up loading from cache. </summary>
        /// <returns> remove handler </returns>
        public Action OnRequestServedFromCache(Action<OnRequestServedFromCacheParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnRequestServedFromCacheParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.requestServedFromCache" : $"Network.requestServedFromCache.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when page is about to send HTTP request. </summary>
        /// <returns> remove handler </returns>
        public Action OnRequestWillBeSent(Action<OnRequestWillBeSentParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnRequestWillBeSentParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.requestWillBeSent" : $"Network.requestWillBeSent.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when resource loading priority is changed </summary>
        /// <returns> remove handler </returns>
        public Action OnResourceChangedPriority(Action<OnResourceChangedPriorityParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnResourceChangedPriorityParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.resourceChangedPriority" : $"Network.resourceChangedPriority.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when a signed exchange was received over the network </summary>
        /// <returns> remove handler </returns>
        public Action OnSignedExchangeReceived(Action<OnSignedExchangeReceivedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnSignedExchangeReceivedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.signedExchangeReceived" : $"Network.signedExchangeReceived.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when HTTP response is available. </summary>
        /// <returns> remove handler </returns>
        public Action OnResponseReceived(Action<OnResponseReceivedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnResponseReceivedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.responseReceived" : $"Network.responseReceived.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when WebSocket is closed. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebSocketClosed(Action<OnWebSocketClosedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebSocketClosedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webSocketClosed" : $"Network.webSocketClosed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired upon WebSocket creation. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebSocketCreated(Action<OnWebSocketCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebSocketCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webSocketCreated" : $"Network.webSocketCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when WebSocket message error occurs. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebSocketFrameError(Action<OnWebSocketFrameErrorParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebSocketFrameErrorParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webSocketFrameError" : $"Network.webSocketFrameError.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when WebSocket message is received. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebSocketFrameReceived(Action<OnWebSocketFrameReceivedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebSocketFrameReceivedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webSocketFrameReceived" : $"Network.webSocketFrameReceived.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when WebSocket message is sent. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebSocketFrameSent(Action<OnWebSocketFrameSentParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebSocketFrameSentParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webSocketFrameSent" : $"Network.webSocketFrameSent.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when WebSocket handshake response becomes available. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebSocketHandshakeResponseReceived(Action<OnWebSocketHandshakeResponseReceivedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebSocketHandshakeResponseReceivedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webSocketHandshakeResponseReceived" : $"Network.webSocketHandshakeResponseReceived.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when WebSocket is about to initiate handshake. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebSocketWillSendHandshakeRequest(Action<OnWebSocketWillSendHandshakeRequestParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebSocketWillSendHandshakeRequestParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webSocketWillSendHandshakeRequest" : $"Network.webSocketWillSendHandshakeRequest.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired upon WebTransport creation. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebTransportCreated(Action<OnWebTransportCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebTransportCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webTransportCreated" : $"Network.webTransportCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when WebTransport handshake is finished. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebTransportConnectionEstablished(Action<OnWebTransportConnectionEstablishedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebTransportConnectionEstablishedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webTransportConnectionEstablished" : $"Network.webTransportConnectionEstablished.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when WebTransport is disposed. </summary>
        /// <returns> remove handler </returns>
        public Action OnWebTransportClosed(Action<OnWebTransportClosedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWebTransportClosedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.webTransportClosed" : $"Network.webTransportClosed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when additional information about a requestWillBeSent event is available from thenetwork stack. Not every requestWillBeSent event will have an additionalrequestWillBeSentExtraInfo fired for it, and there is no guarantee whether requestWillBeSentor requestWillBeSentExtraInfo will be fired first for the same request. </summary>
        /// <returns> remove handler </returns>
        public Action OnRequestWillBeSentExtraInfo(Action<OnRequestWillBeSentExtraInfoParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnRequestWillBeSentExtraInfoParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.requestWillBeSentExtraInfo" : $"Network.requestWillBeSentExtraInfo.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when additional information about a responseReceived event is available from the networkstack. Not every responseReceived event will have an additional responseReceivedExtraInfo forit, and responseReceivedExtraInfo may be fired before or after responseReceived. </summary>
        /// <returns> remove handler </returns>
        public Action OnResponseReceivedExtraInfo(Action<OnResponseReceivedExtraInfoParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnResponseReceivedExtraInfoParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.responseReceivedExtraInfo" : $"Network.responseReceivedExtraInfo.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired exactly once for each Trust Token operation. Depending onthe type of the operation and whether the operation succeeded orfailed, the event is fired before the corresponding request was sentor after the response was received. </summary>
        /// <returns> remove handler </returns>
        public Action OnTrustTokenOperationDone(Action<OnTrustTokenOperationDoneParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnTrustTokenOperationDoneParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.trustTokenOperationDone" : $"Network.trustTokenOperationDone.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired once when parsing the .wbn file has succeeded.The event contains the information about the web bundle contents. </summary>
        /// <returns> remove handler </returns>
        public Action OnSubresourceWebBundleMetadataReceived(Action<OnSubresourceWebBundleMetadataReceivedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnSubresourceWebBundleMetadataReceivedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.subresourceWebBundleMetadataReceived" : $"Network.subresourceWebBundleMetadataReceived.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired once when parsing the .wbn file has failed. </summary>
        /// <returns> remove handler </returns>
        public Action OnSubresourceWebBundleMetadataError(Action<OnSubresourceWebBundleMetadataErrorParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnSubresourceWebBundleMetadataErrorParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.subresourceWebBundleMetadataError" : $"Network.subresourceWebBundleMetadataError.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when handling requests for resources within a .wbn file.Note: this will only be fired for resources that are requested by the webpage. </summary>
        /// <returns> remove handler </returns>
        public Action OnSubresourceWebBundleInnerResponseParsed(Action<OnSubresourceWebBundleInnerResponseParsedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnSubresourceWebBundleInnerResponseParsedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.subresourceWebBundleInnerResponseParsed" : $"Network.subresourceWebBundleInnerResponseParsed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when request for resources within a .wbn file failed. </summary>
        /// <returns> remove handler </returns>
        public Action OnSubresourceWebBundleInnerResponseError(Action<OnSubresourceWebBundleInnerResponseErrorParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnSubresourceWebBundleInnerResponseErrorParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.subresourceWebBundleInnerResponseError" : $"Network.subresourceWebBundleInnerResponseError.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Is sent whenever a new report is added.And after 'enableReportingApi' for all existing reports. </summary>
        /// <returns> remove handler </returns>
        public Action OnReportingApiReportAdded(Action<OnReportingApiReportAddedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnReportingApiReportAddedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.reportingApiReportAdded" : $"Network.reportingApiReportAdded.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnReportingApiReportUpdated(Action<OnReportingApiReportUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnReportingApiReportUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.reportingApiReportUpdated" : $"Network.reportingApiReportUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnReportingApiEndpointsChangedForOrigin(Action<OnReportingApiEndpointsChangedForOriginParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnReportingApiEndpointsChangedForOriginParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Network.reportingApiEndpointsChangedForOrigin" : $"Network.reportingApiEndpointsChangedForOrigin.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Sets a list of content encodings that will be accepted. Empty list means no encoding is accepted. 
        /// </summary>
        public async Task SetAcceptedEncodings(SetAcceptedEncodingsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setAcceptedEncodings", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears accepted encodings set by setAcceptedEncodings 
        /// </summary>
        public async Task ClearAcceptedEncodingsOverride(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.clearAcceptedEncodingsOverride", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Tells whether clearing browser cache is supported. 
        /// </summary>
        public async Task<CanClearBrowserCacheReturn> CanClearBrowserCache(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.canClearBrowserCache", null, sessionId);
            return Convert<CanClearBrowserCacheReturn>(___r);
        }
        /// <summary> 
        /// Tells whether clearing browser cookies is supported. 
        /// </summary>
        public async Task<CanClearBrowserCookiesReturn> CanClearBrowserCookies(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.canClearBrowserCookies", null, sessionId);
            return Convert<CanClearBrowserCookiesReturn>(___r);
        }
        /// <summary> 
        /// Tells whether emulation of network conditions is supported. 
        /// </summary>
        public async Task<CanEmulateNetworkConditionsReturn> CanEmulateNetworkConditions(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.canEmulateNetworkConditions", null, sessionId);
            return Convert<CanEmulateNetworkConditionsReturn>(___r);
        }
        /// <summary> 
        /// Clears browser cache. 
        /// </summary>
        public async Task ClearBrowserCache(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.clearBrowserCache", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears browser cookies. 
        /// </summary>
        public async Task ClearBrowserCookies(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.clearBrowserCookies", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Response to Network.requestIntercepted which either modifies the request to continue with anymodifications, or blocks it, or completes it with the provided response bytes. If a networkfetch occurs as a result which encounters a redirect an additional Network.requestInterceptedevent will be sent with the same InterceptionId.Deprecated, use Fetch.continueRequest, Fetch.fulfillRequest and Fetch.failRequest instead. 
        /// </summary>
        public async Task ContinueInterceptedRequest(ContinueInterceptedRequestParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.continueInterceptedRequest", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Deletes browser cookies with matching name and url or domain/path pair. 
        /// </summary>
        public async Task DeleteCookies(DeleteCookiesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.deleteCookies", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables network tracking, prevents network events from being sent to the client. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Activates emulation of network conditions. 
        /// </summary>
        public async Task EmulateNetworkConditions(EmulateNetworkConditionsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.emulateNetworkConditions", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables network tracking, network events will now be delivered to the client. 
        /// </summary>
        public async Task Enable(EnableParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.enable", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns all browser cookies. Depending on the backend support, will return detailed cookieinformation in the `cookies` field. 
        /// </summary>
        public async Task<GetAllCookiesReturn> GetAllCookies(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.getAllCookies", null, sessionId);
            return Convert<GetAllCookiesReturn>(___r);
        }
        /// <summary> 
        /// Returns the DER-encoded certificate. 
        /// </summary>
        public async Task<GetCertificateReturn> GetCertificate(GetCertificateParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.getCertificate", parameters, sessionId);
            return Convert<GetCertificateReturn>(___r);
        }
        /// <summary> 
        /// Returns all browser cookies for the current URL. Depending on the backend support, will returndetailed cookie information in the `cookies` field. 
        /// </summary>
        public async Task<GetCookiesReturn> GetCookies(GetCookiesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.getCookies", parameters, sessionId);
            return Convert<GetCookiesReturn>(___r);
        }
        /// <summary> 
        /// Returns content served for the given request. 
        /// </summary>
        public async Task<GetResponseBodyReturn> GetResponseBody(GetResponseBodyParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.getResponseBody", parameters, sessionId);
            return Convert<GetResponseBodyReturn>(___r);
        }
        /// <summary> 
        /// Returns post data sent with the request. Returns an error when no data was sent with the request. 
        /// </summary>
        public async Task<GetRequestPostDataReturn> GetRequestPostData(GetRequestPostDataParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.getRequestPostData", parameters, sessionId);
            return Convert<GetRequestPostDataReturn>(___r);
        }
        /// <summary> 
        /// Returns content served for the given currently intercepted request. 
        /// </summary>
        public async Task<GetResponseBodyForInterceptionReturn> GetResponseBodyForInterception(GetResponseBodyForInterceptionParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.getResponseBodyForInterception", parameters, sessionId);
            return Convert<GetResponseBodyForInterceptionReturn>(___r);
        }
        /// <summary> 
        /// Returns a handle to the stream representing the response body. Note that after this command,the intercepted request can't be continued as is -- you either need to cancel it or to providethe response body. The stream only supports sequential read, IO.read will fail if the positionis specified. 
        /// </summary>
        public async Task<TakeResponseBodyForInterceptionAsStreamReturn> TakeResponseBodyForInterceptionAsStream(TakeResponseBodyForInterceptionAsStreamParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.takeResponseBodyForInterceptionAsStream", parameters, sessionId);
            return Convert<TakeResponseBodyForInterceptionAsStreamReturn>(___r);
        }
        /// <summary> 
        /// This method sends a new XMLHttpRequest which is identical to the original one. The followingparameters should be identical: method, url, async, request body, extra headers, withCredentialsattribute, user, password. 
        /// </summary>
        public async Task ReplayXHR(ReplayXHRParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.replayXHR", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Searches for given string in response content. 
        /// </summary>
        public async Task<SearchInResponseBodyReturn> SearchInResponseBody(SearchInResponseBodyParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.searchInResponseBody", parameters, sessionId);
            return Convert<SearchInResponseBodyReturn>(___r);
        }
        /// <summary> 
        /// Blocks URLs from loading. 
        /// </summary>
        public async Task SetBlockedURLs(SetBlockedURLsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setBlockedURLs", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Toggles ignoring of service worker for each request. 
        /// </summary>
        public async Task SetBypassServiceWorker(SetBypassServiceWorkerParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setBypassServiceWorker", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Toggles ignoring cache for each request. If `true`, cache will not be used. 
        /// </summary>
        public async Task SetCacheDisabled(SetCacheDisabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setCacheDisabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets a cookie with the given cookie data; may overwrite equivalent cookies if they exist. 
        /// </summary>
        public async Task<SetCookieReturn> SetCookie(SetCookieParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setCookie", parameters, sessionId);
            return Convert<SetCookieReturn>(___r);
        }
        /// <summary> 
        /// Sets given cookies. 
        /// </summary>
        public async Task SetCookies(SetCookiesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setCookies", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Specifies whether to always send extra HTTP headers with the requests from this page. 
        /// </summary>
        public async Task SetExtraHTTPHeaders(SetExtraHTTPHeadersParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setExtraHTTPHeaders", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Specifies whether to attach a page script stack id in requests 
        /// </summary>
        public async Task SetAttachDebugStack(SetAttachDebugStackParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setAttachDebugStack", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets the requests to intercept that match the provided patterns and optionally resource types.Deprecated, please use Fetch.enable instead. 
        /// </summary>
        public async Task SetRequestInterception(SetRequestInterceptionParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setRequestInterception", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Allows overriding user agent with the given string. 
        /// </summary>
        public async Task SetUserAgentOverride(SetUserAgentOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.setUserAgentOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns information about the COEP/COOP isolation status. 
        /// </summary>
        public async Task<GetSecurityIsolationStatusReturn> GetSecurityIsolationStatus(GetSecurityIsolationStatusParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.getSecurityIsolationStatus", parameters, sessionId);
            return Convert<GetSecurityIsolationStatusReturn>(___r);
        }
        /// <summary> 
        /// Enables tracking for the Reporting API, events generated by the Reporting API will now be delivered to the client.Enabling triggers 'reportingApiReportAdded' for all existing reports. 
        /// </summary>
        public async Task EnableReportingApi(EnableReportingApiParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.enableReportingApi", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Fetches the resource and returns the content. 
        /// </summary>
        public async Task<LoadNetworkResourceReturn> LoadNetworkResource(LoadNetworkResourceParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Network.loadNetworkResource", parameters, sessionId);
            return Convert<LoadNetworkResourceReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class HeadersType
        {
            
        }
        public class ResourceTimingType
        {
            
            /// <summary> Timing's requestTime is a baseline in seconds, while the other numbers are ticks inmilliseconds relatively to this requestTime. </summary>
            public double requestTime;
            /// <summary> Started resolving proxy. </summary>
            public double proxyStart;
            /// <summary> Finished resolving proxy. </summary>
            public double proxyEnd;
            /// <summary> Started DNS address resolve. </summary>
            public double dnsStart;
            /// <summary> Finished DNS address resolve. </summary>
            public double dnsEnd;
            /// <summary> Started connecting to the remote host. </summary>
            public double connectStart;
            /// <summary> Connected to the remote host. </summary>
            public double connectEnd;
            /// <summary> Started SSL handshake. </summary>
            public double sslStart;
            /// <summary> Finished SSL handshake. </summary>
            public double sslEnd;
            /// <summary> Started running ServiceWorker. </summary>
            public double workerStart;
            /// <summary> Finished Starting ServiceWorker. </summary>
            public double workerReady;
            /// <summary> Started fetch event. </summary>
            public double workerFetchStart;
            /// <summary> Settled fetch event respondWith promise. </summary>
            public double workerRespondWithSettled;
            /// <summary> Started sending request. </summary>
            public double sendStart;
            /// <summary> Finished sending request. </summary>
            public double sendEnd;
            /// <summary> Time the server started pushing request. </summary>
            public double pushStart;
            /// <summary> Time the server finished pushing request. </summary>
            public double pushEnd;
            /// <summary> Finished receiving response headers. </summary>
            public double receiveHeadersEnd;
        }
        public class PostDataEntryType
        {
            
            /// <summary>  </summary>
            public string bytes;
        }
        public class RequestType
        {
            
            /// <summary> Request URL (without fragment). </summary>
            public string url;
            /// <summary> Fragment of the requested URL starting with hash, if present. </summary>
            public string urlFragment;
            /// <summary> HTTP request method. </summary>
            public string method;
            /// <summary> HTTP request headers. </summary>
            public Network.HeadersType headers;
            /// <summary> HTTP POST request data. </summary>
            public string postData;
            /// <summary> True when the request has POST data. Note that postData might still be omitted when this flag is true when the data is too long. </summary>
            public bool hasPostData;
            /// <summary> Request body elements. This will be converted from base64 to binary </summary>
            public object[] postDataEntries;
            /// <summary> The mixed content type of the request. </summary>
            public string mixedContentType;
            /// <summary> Priority of the resource request at the time request is sent. </summary>
            public string initialPriority;
            /// <summary> The referrer policy of the request, as defined in https://www.w3.org/TR/referrer-policy/ </summary>
            public string referrerPolicy;
            /// <summary> Whether is loaded via link preload. </summary>
            public bool isLinkPreload;
            /// <summary> Set for requests when the TrustToken API is used. Contains the parameterspassed by the developer (e.g. via "fetch") as understood by the backend. </summary>
            public Network.TrustTokenParamsType trustTokenParams;
            /// <summary> True if this resource request is considered to be the 'same site' as therequest correspondinfg to the main frame. </summary>
            public bool isSameSite;
        }
        public class SignedCertificateTimestampType
        {
            
            /// <summary> Validation status. </summary>
            public string status;
            /// <summary> Origin. </summary>
            public string origin;
            /// <summary> Log name / description. </summary>
            public string logDescription;
            /// <summary> Log ID. </summary>
            public string logId;
            /// <summary> Issuance date. Unlike TimeSinceEpoch, this contains the number ofmilliseconds since January 1, 1970, UTC, not the number of seconds. </summary>
            public double timestamp;
            /// <summary> Hash algorithm. </summary>
            public string hashAlgorithm;
            /// <summary> Signature algorithm. </summary>
            public string signatureAlgorithm;
            /// <summary> Signature data. </summary>
            public string signatureData;
        }
        public class SecurityDetailsType
        {
            
            /// <summary> Protocol name (e.g. "TLS 1.2" or "QUIC"). </summary>
            public string protocol;
            /// <summary> Key Exchange used by the connection, or the empty string if not applicable. </summary>
            public string keyExchange;
            /// <summary> (EC)DH group used by the connection, if applicable. </summary>
            public string keyExchangeGroup;
            /// <summary> Cipher name. </summary>
            public string cipher;
            /// <summary> TLS MAC. Note that AEAD ciphers do not have separate MACs. </summary>
            public string mac;
            /// <summary> Certificate ID value. </summary>
            public int certificateId;
            /// <summary> Certificate subject name. </summary>
            public string subjectName;
            /// <summary> Subject Alternative Name (SAN) DNS names and IP addresses. </summary>
            public object[] sanList;
            /// <summary> Name of the issuing CA. </summary>
            public string issuer;
            /// <summary> Certificate valid from date. </summary>
            public double validFrom;
            /// <summary> Certificate valid to (expiration) date </summary>
            public double validTo;
            /// <summary> List of signed certificate timestamps (SCTs). </summary>
            public object[] signedCertificateTimestampList;
            /// <summary> Whether the request complied with Certificate Transparency policy </summary>
            public string certificateTransparencyCompliance;
        }
        public class CorsErrorStatusType
        {
            
            /// <summary>  </summary>
            public string corsError;
            /// <summary>  </summary>
            public string failedParameter;
        }
        public class TrustTokenParamsType
        {
            
            /// <summary>  </summary>
            public string type;
            /// <summary> Only set for "token-redemption" type and determine whetherto request a fresh SRR or use a still valid cached SRR. </summary>
            public string refreshPolicy;
            /// <summary> Origins of issuers from whom to request tokens or redemptionrecords. </summary>
            public object[] issuers;
        }
        public class ResponseType
        {
            
            /// <summary> Response URL. This URL can be different from CachedResource.url in case of redirect. </summary>
            public string url;
            /// <summary> HTTP response status code. </summary>
            public int status;
            /// <summary> HTTP response status text. </summary>
            public string statusText;
            /// <summary> HTTP response headers. </summary>
            public Network.HeadersType headers;
            /// <summary> HTTP response headers text. This has been replaced by the headers in Network.responseReceivedExtraInfo. </summary>
            public string headersText;
            /// <summary> Resource mimeType as determined by the browser. </summary>
            public string mimeType;
            /// <summary> Refined HTTP request headers that were actually transmitted over the network. </summary>
            public Network.HeadersType requestHeaders;
            /// <summary> HTTP request headers text. This has been replaced by the headers in Network.requestWillBeSentExtraInfo. </summary>
            public string requestHeadersText;
            /// <summary> Specifies whether physical connection was actually reused for this request. </summary>
            public bool connectionReused;
            /// <summary> Physical connection id that was actually used for this request. </summary>
            public double connectionId;
            /// <summary> Remote IP address. </summary>
            public string remoteIPAddress;
            /// <summary> Remote port. </summary>
            public int remotePort;
            /// <summary> Specifies that the request was served from the disk cache. </summary>
            public bool fromDiskCache;
            /// <summary> Specifies that the request was served from the ServiceWorker. </summary>
            public bool fromServiceWorker;
            /// <summary> Specifies that the request was served from the prefetch cache. </summary>
            public bool fromPrefetchCache;
            /// <summary> Total number of bytes received for this request so far. </summary>
            public double encodedDataLength;
            /// <summary> Timing information for the given request. </summary>
            public Network.ResourceTimingType timing;
            /// <summary> Response source of response from ServiceWorker. </summary>
            public string serviceWorkerResponseSource;
            /// <summary> The time at which the returned response was generated. </summary>
            public double responseTime;
            /// <summary> Cache Storage Cache Name. </summary>
            public string cacheStorageCacheName;
            /// <summary> Protocol used to fetch this request. </summary>
            public string protocol;
            /// <summary> Security state of the request resource. </summary>
            public string securityState;
            /// <summary> Security details for the request. </summary>
            public Network.SecurityDetailsType securityDetails;
        }
        public class WebSocketRequestType
        {
            
            /// <summary> HTTP request headers. </summary>
            public Network.HeadersType headers;
        }
        public class WebSocketResponseType
        {
            
            /// <summary> HTTP response status code. </summary>
            public int status;
            /// <summary> HTTP response status text. </summary>
            public string statusText;
            /// <summary> HTTP response headers. </summary>
            public Network.HeadersType headers;
            /// <summary> HTTP response headers text. </summary>
            public string headersText;
            /// <summary> HTTP request headers. </summary>
            public Network.HeadersType requestHeaders;
            /// <summary> HTTP request headers text. </summary>
            public string requestHeadersText;
        }
        public class WebSocketFrameType
        {
            
            /// <summary> WebSocket message opcode. </summary>
            public double opcode;
            /// <summary> WebSocket message mask. </summary>
            public bool mask;
            /// <summary> WebSocket message payload data.If the opcode is 1, this is a text message and payloadData is a UTF-8 string.If the opcode isn't 1, then payloadData is a base64 encoded string representing binary data. </summary>
            public string payloadData;
        }
        public class CachedResourceType
        {
            
            /// <summary> Resource URL. This is the url of the original network request. </summary>
            public string url;
            /// <summary> Type of this resource. </summary>
            public string type;
            /// <summary> Cached response data. </summary>
            public Network.ResponseType response;
            /// <summary> Cached response body size. </summary>
            public double bodySize;
        }
        public class InitiatorType
        {
            
            /// <summary> Type of this initiator. </summary>
            public string type;
            /// <summary> Initiator JavaScript stack trace, set for Script only. </summary>
            public Runtime.StackTraceType stack;
            /// <summary> Initiator URL, set for Parser type or for Script type (when script is importing module) or for SignedExchange type. </summary>
            public string url;
            /// <summary> Initiator line number, set for Parser type or for Script type (when script is importingmodule) (0-based). </summary>
            public double lineNumber;
            /// <summary> Initiator column number, set for Parser type or for Script type (when script is importingmodule) (0-based). </summary>
            public double columnNumber;
            /// <summary> Set if another request triggered this request (e.g. preflight). </summary>
            public string requestId;
        }
        public class CookieType
        {
            
            /// <summary> Cookie name. </summary>
            public string name;
            /// <summary> Cookie value. </summary>
            public string value;
            /// <summary> Cookie domain. </summary>
            public string domain;
            /// <summary> Cookie path. </summary>
            public string path;
            /// <summary> Cookie expiration date as the number of seconds since the UNIX epoch. </summary>
            public double expires;
            /// <summary> Cookie size. </summary>
            public int size;
            /// <summary> True if cookie is http-only. </summary>
            public bool httpOnly;
            /// <summary> True if cookie is secure. </summary>
            public bool secure;
            /// <summary> True in case of session cookie. </summary>
            public bool session;
            /// <summary> Cookie SameSite type. </summary>
            public string sameSite;
            /// <summary> Cookie Priority </summary>
            public string priority;
            /// <summary> True if cookie is SameParty. </summary>
            public bool sameParty;
            /// <summary> Cookie source scheme type. </summary>
            public string sourceScheme;
            /// <summary> Cookie source port. Valid values are {-1, [1, 65535]}, -1 indicates an unspecified port.An unspecified port value allows protocol clients to emulate legacy cookie scope for the port.This is a temporary ability and it will be removed in the future. </summary>
            public int sourcePort;
            /// <summary> Cookie partition key. The site of the top-level URL the browser was visiting at the startof the request to the endpoint that set the cookie. </summary>
            public string partitionKey;
            /// <summary> True if cookie partition key is opaque. </summary>
            public bool partitionKeyOpaque;
        }
        public class BlockedSetCookieWithReasonType
        {
            
            /// <summary> The reason(s) this cookie was blocked. </summary>
            public object[] blockedReasons;
            /// <summary> The string representing this individual cookie as it would appear in the header.This is not the entire "cookie" or "set-cookie" header which could have multiple cookies. </summary>
            public string cookieLine;
            /// <summary> The cookie object which represents the cookie which was not stored. It is optional becausesometimes complete cookie information is not available, such as in the case of parsingerrors. </summary>
            public Network.CookieType cookie;
        }
        public class BlockedCookieWithReasonType
        {
            
            /// <summary> The reason(s) the cookie was blocked. </summary>
            public object[] blockedReasons;
            /// <summary> The cookie object representing the cookie which was not sent. </summary>
            public Network.CookieType cookie;
        }
        public class CookieParamType
        {
            
            /// <summary> Cookie name. </summary>
            public string name;
            /// <summary> Cookie value. </summary>
            public string value;
            /// <summary> The request-URI to associate with the setting of the cookie. This value can affect thedefault domain, path, source port, and source scheme values of the created cookie. </summary>
            public string url;
            /// <summary> Cookie domain. </summary>
            public string domain;
            /// <summary> Cookie path. </summary>
            public string path;
            /// <summary> True if cookie is secure. </summary>
            public bool secure;
            /// <summary> True if cookie is http-only. </summary>
            public bool httpOnly;
            /// <summary> Cookie SameSite type. </summary>
            public string sameSite;
            /// <summary> Cookie expiration date, session cookie if not set </summary>
            public double expires;
            /// <summary> Cookie Priority. </summary>
            public string priority;
            /// <summary> True if cookie is SameParty. </summary>
            public bool sameParty;
            /// <summary> Cookie source scheme type. </summary>
            public string sourceScheme;
            /// <summary> Cookie source port. Valid values are {-1, [1, 65535]}, -1 indicates an unspecified port.An unspecified port value allows protocol clients to emulate legacy cookie scope for the port.This is a temporary ability and it will be removed in the future. </summary>
            public int sourcePort;
            /// <summary> Cookie partition key. The site of the top-level URL the browser was visiting at the startof the request to the endpoint that set the cookie.If not set, the cookie will be set as not partitioned. </summary>
            public string partitionKey;
        }
        public class AuthChallengeType
        {
            
            /// <summary> Source of the authentication challenge. </summary>
            public string source;
            /// <summary> Origin of the challenger. </summary>
            public string origin;
            /// <summary> The authentication scheme used, such as basic or digest </summary>
            public string scheme;
            /// <summary> The realm of the challenge. May be empty. </summary>
            public string realm;
        }
        public class AuthChallengeResponseType
        {
            
            /// <summary> The decision on what to do in response to the authorization challenge.  Default meansdeferring to the default behavior of the net stack, which will likely either the Cancelauthentication or display a popup dialog box. </summary>
            public string response;
            /// <summary> The username to provide, possibly empty. Should only be set if response isProvideCredentials. </summary>
            public string username;
            /// <summary> The password to provide, possibly empty. Should only be set if response isProvideCredentials. </summary>
            public string password;
        }
        public class RequestPatternType
        {
            
            /// <summary> Wildcards (`'*'` -> zero or more, `'?'` -> exactly one) are allowed. Escape character isbackslash. Omitting is equivalent to `"*"`. </summary>
            public string urlPattern;
            /// <summary> If set, only requests for matching resource types will be intercepted. </summary>
            public string resourceType;
            /// <summary> Stage at which to begin intercepting requests. Default is Request. </summary>
            public string interceptionStage;
        }
        public class SignedExchangeSignatureType
        {
            
            /// <summary> Signed exchange signature label. </summary>
            public string label;
            /// <summary> The hex string of signed exchange signature. </summary>
            public string signature;
            /// <summary> Signed exchange signature integrity. </summary>
            public string integrity;
            /// <summary> Signed exchange signature cert Url. </summary>
            public string certUrl;
            /// <summary> The hex string of signed exchange signature cert sha256. </summary>
            public string certSha256;
            /// <summary> Signed exchange signature validity Url. </summary>
            public string validityUrl;
            /// <summary> Signed exchange signature date. </summary>
            public int date;
            /// <summary> Signed exchange signature expires. </summary>
            public int expires;
            /// <summary> The encoded certificates. </summary>
            public object[] certificates;
        }
        public class SignedExchangeHeaderType
        {
            
            /// <summary> Signed exchange request URL. </summary>
            public string requestUrl;
            /// <summary> Signed exchange response code. </summary>
            public int responseCode;
            /// <summary> Signed exchange response headers. </summary>
            public Network.HeadersType responseHeaders;
            /// <summary> Signed exchange response signature. </summary>
            public object[] signatures;
            /// <summary> Signed exchange header integrity hash in the form of "sha256-<base64-hash-value>". </summary>
            public string headerIntegrity;
        }
        public class SignedExchangeErrorType
        {
            
            /// <summary> Error message. </summary>
            public string message;
            /// <summary> The index of the signature which caused the error. </summary>
            public int signatureIndex;
            /// <summary> The field which caused the error. </summary>
            public string errorField;
        }
        public class SignedExchangeInfoType
        {
            
            /// <summary> The outer response of signed HTTP exchange which was received from network. </summary>
            public Network.ResponseType outerResponse;
            /// <summary> Information about the signed exchange header. </summary>
            public Network.SignedExchangeHeaderType header;
            /// <summary> Security details for the signed exchange header. </summary>
            public Network.SecurityDetailsType securityDetails;
            /// <summary> Errors occurred while handling the signed exchagne. </summary>
            public object[] errors;
        }
        public class ConnectTimingType
        {
            
            /// <summary> Timing's requestTime is a baseline in seconds, while the other numbers are ticks inmilliseconds relatively to this requestTime. Matches ResourceTiming's requestTime forthe same request (but not for redirected requests). </summary>
            public double requestTime;
        }
        public class ClientSecurityStateType
        {
            
            /// <summary>  </summary>
            public bool initiatorIsSecureContext;
            /// <summary>  </summary>
            public string initiatorIPAddressSpace;
            /// <summary>  </summary>
            public string privateNetworkRequestPolicy;
        }
        public class CrossOriginOpenerPolicyStatusType
        {
            
            /// <summary>  </summary>
            public string value;
            /// <summary>  </summary>
            public string reportOnlyValue;
            /// <summary>  </summary>
            public string reportingEndpoint;
            /// <summary>  </summary>
            public string reportOnlyReportingEndpoint;
        }
        public class CrossOriginEmbedderPolicyStatusType
        {
            
            /// <summary>  </summary>
            public string value;
            /// <summary>  </summary>
            public string reportOnlyValue;
            /// <summary>  </summary>
            public string reportingEndpoint;
            /// <summary>  </summary>
            public string reportOnlyReportingEndpoint;
        }
        public class SecurityIsolationStatusType
        {
            
            /// <summary>  </summary>
            public Network.CrossOriginOpenerPolicyStatusType coop;
            /// <summary>  </summary>
            public Network.CrossOriginEmbedderPolicyStatusType coep;
        }
        public class ReportingApiReportType
        {
            
            /// <summary>  </summary>
            public string id;
            /// <summary> The URL of the document that triggered the report. </summary>
            public string initiatorUrl;
            /// <summary> The name of the endpoint group that should be used to deliver the report. </summary>
            public string destination;
            /// <summary> The type of the report (specifies the set of data that is contained in the report body). </summary>
            public string type;
            /// <summary> When the report was generated. </summary>
            public double timestamp;
            /// <summary> How many uploads deep the related request was. </summary>
            public int depth;
            /// <summary> The number of delivery attempts made so far, not including an active attempt. </summary>
            public int completedAttempts;
            /// <summary>  </summary>
            public object body;
            /// <summary>  </summary>
            public string status;
        }
        public class ReportingApiEndpointType
        {
            
            /// <summary> The URL of the endpoint to which reports may be delivered. </summary>
            public string url;
            /// <summary> Name of the endpoint group. </summary>
            public string groupName;
        }
        public class LoadNetworkResourcePageResultType
        {
            
            /// <summary>  </summary>
            public bool success;
            /// <summary> Optional values used for error reporting. </summary>
            public double netError;
            /// <summary>  </summary>
            public string netErrorName;
            /// <summary>  </summary>
            public double httpStatusCode;
            /// <summary> If successful, one of the following two fields holds the result. </summary>
            public string stream;
            /// <summary> Response headers. </summary>
            public Network.HeadersType headers;
        }
        public class LoadNetworkResourceOptionsType
        {
            
            /// <summary>  </summary>
            public bool disableCache;
            /// <summary>  </summary>
            public bool includeCredentials;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnDataReceivedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] Data chunk length. </summary>
            public int dataLength;
            /// <summary> [Require] Actual bytes received (might be less than dataLength for compressed encodings). </summary>
            public int encodedDataLength;
        }
        public class OnEventSourceMessageReceivedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] Message type. </summary>
            public string eventName;
            /// <summary> [Require] Message identifier. </summary>
            public string eventId;
            /// <summary> [Require] Message content. </summary>
            public string data;
        }
        public class OnLoadingFailedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] Resource type. </summary>
            public string type;
            /// <summary> [Require] User friendly error message. </summary>
            public string errorText;
            /// <summary> [Optional] True if loading was canceled. </summary>
            public bool canceled;
            /// <summary> [Optional] The reason why loading was blocked, if any. </summary>
            public string blockedReason;
            /// <summary> [Optional] The reason why loading was blocked by CORS, if any. </summary>
            public Network.CorsErrorStatusType corsErrorStatus;
        }
        public class OnLoadingFinishedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] Total number of bytes received for this request. </summary>
            public double encodedDataLength;
            /// <summary> [Optional] Set when 1) response was blocked by Cross-Origin Read Blocking and also2) this needs to be reported to the DevTools console. </summary>
            public bool shouldReportCorbBlocking;
        }
        public class OnRequestInterceptedParameters
        {
            
            /// <summary> [Require] Each request the page makes will have a unique id, however if any redirects are encounteredwhile processing that fetch, they will be reported with the same id as the original fetch.Likewise if HTTP authentication is needed then the same fetch id will be used. </summary>
            public string interceptionId;
            /// <summary> [Require]  </summary>
            public Network.RequestType request;
            /// <summary> [Require] The id of the frame that initiated the request. </summary>
            public string frameId;
            /// <summary> [Require] How the requested resource will be used. </summary>
            public string resourceType;
            /// <summary> [Require] Whether this is a navigation request, which can abort the navigation completely. </summary>
            public bool isNavigationRequest;
            /// <summary> [Optional] Set if the request is a navigation that will result in a download.Only present after response is received from the server (i.e. HeadersReceived stage). </summary>
            public bool isDownload;
            /// <summary> [Optional] Redirect location, only sent if a redirect was intercepted. </summary>
            public string redirectUrl;
            /// <summary> [Optional] Details of the Authorization Challenge encountered. If this is set thencontinueInterceptedRequest must contain an authChallengeResponse. </summary>
            public Network.AuthChallengeType authChallenge;
            /// <summary> [Optional] Response error if intercepted at response stage or if redirect occurred while interceptingrequest. </summary>
            public string responseErrorReason;
            /// <summary> [Optional] Response code if intercepted at response stage or if redirect occurred while interceptingrequest or auth retry occurred. </summary>
            public int responseStatusCode;
            /// <summary> [Optional] Response headers if intercepted at the response stage or if redirect occurred whileintercepting request or auth retry occurred. </summary>
            public Network.HeadersType responseHeaders;
            /// <summary> [Optional] If the intercepted request had a corresponding requestWillBeSent event fired for it, thenthis requestId will be the same as the requestId present in the requestWillBeSent event. </summary>
            public string requestId;
        }
        public class OnRequestServedFromCacheParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
        }
        public class OnRequestWillBeSentParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Loader identifier. Empty string if the request is fetched from worker. </summary>
            public string loaderId;
            /// <summary> [Require] URL of the document this request is loaded for. </summary>
            public string documentURL;
            /// <summary> [Require] Request data. </summary>
            public Network.RequestType request;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] Timestamp. </summary>
            public double wallTime;
            /// <summary> [Require] Request initiator. </summary>
            public Network.InitiatorType initiator;
            /// <summary> [Require] In the case that redirectResponse is populated, this flag indicates whetherrequestWillBeSentExtraInfo and responseReceivedExtraInfo events will be or were emittedfor the request which was just redirected. </summary>
            public bool redirectHasExtraInfo;
            /// <summary> [Optional] Redirect response data. </summary>
            public Network.ResponseType redirectResponse;
            /// <summary> [Optional] Type of this resource. </summary>
            public string type;
            /// <summary> [Optional] Frame identifier. </summary>
            public string frameId;
            /// <summary> [Optional] Whether the request is initiated by a user gesture. Defaults to false. </summary>
            public bool hasUserGesture;
        }
        public class OnResourceChangedPriorityParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] New priority </summary>
            public string newPriority;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
        }
        public class OnSignedExchangeReceivedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Information about the signed exchange response. </summary>
            public Network.SignedExchangeInfoType info;
        }
        public class OnResponseReceivedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Loader identifier. Empty string if the request is fetched from worker. </summary>
            public string loaderId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] Resource type. </summary>
            public string type;
            /// <summary> [Require] Response data. </summary>
            public Network.ResponseType response;
            /// <summary> [Require] Indicates whether requestWillBeSentExtraInfo and responseReceivedExtraInfo events will beor were emitted for this request. </summary>
            public bool hasExtraInfo;
            /// <summary> [Optional] Frame identifier. </summary>
            public string frameId;
        }
        public class OnWebSocketClosedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
        }
        public class OnWebSocketCreatedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] WebSocket request URL. </summary>
            public string url;
            /// <summary> [Optional] Request initiator. </summary>
            public Network.InitiatorType initiator;
        }
        public class OnWebSocketFrameErrorParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] WebSocket error message. </summary>
            public string errorMessage;
        }
        public class OnWebSocketFrameReceivedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] WebSocket response data. </summary>
            public Network.WebSocketFrameType response;
        }
        public class OnWebSocketFrameSentParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] WebSocket response data. </summary>
            public Network.WebSocketFrameType response;
        }
        public class OnWebSocketHandshakeResponseReceivedParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] WebSocket response data. </summary>
            public Network.WebSocketResponseType response;
        }
        public class OnWebSocketWillSendHandshakeRequestParameters
        {
            
            /// <summary> [Require] Request identifier. </summary>
            public string requestId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Require] UTC Timestamp. </summary>
            public double wallTime;
            /// <summary> [Require] WebSocket request data. </summary>
            public Network.WebSocketRequestType request;
        }
        public class OnWebTransportCreatedParameters
        {
            
            /// <summary> [Require] WebTransport identifier. </summary>
            public string transportId;
            /// <summary> [Require] WebTransport request URL. </summary>
            public string url;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
            /// <summary> [Optional] Request initiator. </summary>
            public Network.InitiatorType initiator;
        }
        public class OnWebTransportConnectionEstablishedParameters
        {
            
            /// <summary> [Require] WebTransport identifier. </summary>
            public string transportId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
        }
        public class OnWebTransportClosedParameters
        {
            
            /// <summary> [Require] WebTransport identifier. </summary>
            public string transportId;
            /// <summary> [Require] Timestamp. </summary>
            public double timestamp;
        }
        public class OnRequestWillBeSentExtraInfoParameters
        {
            
            /// <summary> [Require] Request identifier. Used to match this information to an existing requestWillBeSent event. </summary>
            public string requestId;
            /// <summary> [Require] A list of cookies potentially associated to the requested URL. This includes both cookies sent withthe request and the ones not sent; the latter are distinguished by having blockedReason field set. </summary>
            public object[] associatedCookies;
            /// <summary> [Require] Raw request headers as they will be sent over the wire. </summary>
            public Network.HeadersType headers;
            /// <summary> [Require] Connection timing information for the request. </summary>
            public Network.ConnectTimingType connectTiming;
            /// <summary> [Optional] The client security state set for the request. </summary>
            public Network.ClientSecurityStateType clientSecurityState;
        }
        public class OnResponseReceivedExtraInfoParameters
        {
            
            /// <summary> [Require] Request identifier. Used to match this information to another responseReceived event. </summary>
            public string requestId;
            /// <summary> [Require] A list of cookies which were not stored from the response along with the correspondingreasons for blocking. The cookies here may not be valid due to syntax errors, whichare represented by the invalid cookie line string instead of a proper cookie. </summary>
            public object[] blockedCookies;
            /// <summary> [Require] Raw response headers as they were received over the wire. </summary>
            public Network.HeadersType headers;
            /// <summary> [Require] The IP address space of the resource. The address space can only be determined once the transportestablished the connection, so we can't send it in `requestWillBeSentExtraInfo`. </summary>
            public string resourceIPAddressSpace;
            /// <summary> [Require] The status code of the response. This is useful in cases the request failed and no responseReceivedevent is triggered, which is the case for, e.g., CORS errors. This is also the correct status codefor cached requests, where the status in responseReceived is a 200 and this will be 304. </summary>
            public int statusCode;
            /// <summary> [Optional] Raw response header text as it was received over the wire. The raw text may not always beavailable, such as in the case of HTTP/2 or QUIC. </summary>
            public string headersText;
        }
        public class OnTrustTokenOperationDoneParameters
        {
            
            /// <summary> [Require] Detailed success or error status of the operation.'AlreadyExists' also signifies a successful operation, as the resultof the operation already exists und thus, the operation was abortpreemptively (e.g. a cache hit). </summary>
            public string status;
            /// <summary> [Require]  </summary>
            public string type;
            /// <summary> [Require]  </summary>
            public string requestId;
            /// <summary> [Optional] Top level origin. The context in which the operation was attempted. </summary>
            public string topLevelOrigin;
            /// <summary> [Optional] Origin of the issuer in case of a "Issuance" or "Redemption" operation. </summary>
            public string issuerOrigin;
            /// <summary> [Optional] The number of obtained Trust Tokens on a successful "Issuance" operation. </summary>
            public int issuedTokenCount;
        }
        public class OnSubresourceWebBundleMetadataReceivedParameters
        {
            
            /// <summary> [Require] Request identifier. Used to match this information to another event. </summary>
            public string requestId;
            /// <summary> [Require] A list of URLs of resources in the subresource Web Bundle. </summary>
            public object[] urls;
        }
        public class OnSubresourceWebBundleMetadataErrorParameters
        {
            
            /// <summary> [Require] Request identifier. Used to match this information to another event. </summary>
            public string requestId;
            /// <summary> [Require] Error message </summary>
            public string errorMessage;
        }
        public class OnSubresourceWebBundleInnerResponseParsedParameters
        {
            
            /// <summary> [Require] Request identifier of the subresource request </summary>
            public string innerRequestId;
            /// <summary> [Require] URL of the subresource resource. </summary>
            public string innerRequestURL;
            /// <summary> [Optional] Bundle request identifier. Used to match this information to another event.This made be absent in case when the instrumentation was enabled onlyafter webbundle was parsed. </summary>
            public string bundleRequestId;
        }
        public class OnSubresourceWebBundleInnerResponseErrorParameters
        {
            
            /// <summary> [Require] Request identifier of the subresource request </summary>
            public string innerRequestId;
            /// <summary> [Require] URL of the subresource resource. </summary>
            public string innerRequestURL;
            /// <summary> [Require] Error message </summary>
            public string errorMessage;
            /// <summary> [Optional] Bundle request identifier. Used to match this information to another event.This made be absent in case when the instrumentation was enabled onlyafter webbundle was parsed. </summary>
            public string bundleRequestId;
        }
        public class OnReportingApiReportAddedParameters
        {
            
            /// <summary> [Require]  </summary>
            public Network.ReportingApiReportType report;
        }
        public class OnReportingApiReportUpdatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public Network.ReportingApiReportType report;
        }
        public class OnReportingApiEndpointsChangedForOriginParameters
        {
            
            /// <summary> [Require] Origin of the document(s) which configured the endpoints. </summary>
            public string origin;
            /// <summary> [Require]  </summary>
            public object[] endpoints;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class SetAcceptedEncodingsParameters
        {
            
            /// <summary> [Require] List of accepted content encodings. </summary>
            public object[] encodings;
        }
        public class ContinueInterceptedRequestParameters
        {
            
            /// <summary> [Require]  </summary>
            public string interceptionId;
            /// <summary> [Optional] If set this causes the request to fail with the given reason. Passing `Aborted` for requestsmarked with `isNavigationRequest` also cancels the navigation. Must not be set in responseto an authChallenge. </summary>
            public string errorReason;
            /// <summary> [Optional] If set the requests completes using with the provided base64 encoded raw response, includingHTTP status line and headers etc... Must not be set in response to an authChallenge. (Encoded as a base64 string when passed over JSON) </summary>
            public string rawResponse;
            /// <summary> [Optional] If set the request url will be modified in a way that's not observable by page. Must not beset in response to an authChallenge. </summary>
            public string url;
            /// <summary> [Optional] If set this allows the request method to be overridden. Must not be set in response to anauthChallenge. </summary>
            public string method;
            /// <summary> [Optional] If set this allows postData to be set. Must not be set in response to an authChallenge. </summary>
            public string postData;
            /// <summary> [Optional] If set this allows the request headers to be changed. Must not be set in response to anauthChallenge. </summary>
            public Network.HeadersType headers;
            /// <summary> [Optional] Response to a requestIntercepted with an authChallenge. Must not be set otherwise. </summary>
            public Network.AuthChallengeResponseType authChallengeResponse;
        }
        public class DeleteCookiesParameters
        {
            
            /// <summary> [Require] Name of the cookies to remove. </summary>
            public string name;
            /// <summary> [Optional] If specified, deletes all the cookies with the given name where domain and path matchprovided URL. </summary>
            public string url;
            /// <summary> [Optional] If specified, deletes only cookies with the exact domain. </summary>
            public string domain;
            /// <summary> [Optional] If specified, deletes only cookies with the exact path. </summary>
            public string path;
        }
        public class EmulateNetworkConditionsParameters
        {
            
            /// <summary> [Require] True to emulate internet disconnection. </summary>
            public bool offline;
            /// <summary> [Require] Minimum latency from request sent to response headers received (ms). </summary>
            public double latency;
            /// <summary> [Require] Maximal aggregated download throughput (bytes/sec). -1 disables download throttling. </summary>
            public double downloadThroughput;
            /// <summary> [Require] Maximal aggregated upload throughput (bytes/sec).  -1 disables upload throttling. </summary>
            public double uploadThroughput;
            /// <summary> [Optional] Connection type if known. </summary>
            public string connectionType;
        }
        public class EnableParameters
        {
            
            /// <summary> [Optional] Buffer size in bytes to use when preserving network payloads (XHRs, etc). </summary>
            public int maxTotalBufferSize;
            /// <summary> [Optional] Per-resource buffer size in bytes to use when preserving network payloads (XHRs, etc). </summary>
            public int maxResourceBufferSize;
            /// <summary> [Optional] Longest post body size (in bytes) that would be included in requestWillBeSent notification </summary>
            public int maxPostDataSize;
        }
        public class GetCertificateParameters
        {
            
            /// <summary> [Require] Origin to get certificate for. </summary>
            public string origin;
        }
        public class GetCookiesParameters
        {
            
            /// <summary> [Optional] The list of URLs for which applicable cookies will be fetched.If not specified, it's assumed to be set to the list containingthe URLs of the page and all of its subframes. </summary>
            public object[] urls;
        }
        public class GetResponseBodyParameters
        {
            
            /// <summary> [Require] Identifier of the network request to get content for. </summary>
            public string requestId;
        }
        public class GetRequestPostDataParameters
        {
            
            /// <summary> [Require] Identifier of the network request to get content for. </summary>
            public string requestId;
        }
        public class GetResponseBodyForInterceptionParameters
        {
            
            /// <summary> [Require] Identifier for the intercepted request to get body for. </summary>
            public string interceptionId;
        }
        public class TakeResponseBodyForInterceptionAsStreamParameters
        {
            
            /// <summary> [Require]  </summary>
            public string interceptionId;
        }
        public class ReplayXHRParameters
        {
            
            /// <summary> [Require] Identifier of XHR to replay. </summary>
            public string requestId;
        }
        public class SearchInResponseBodyParameters
        {
            
            /// <summary> [Require] Identifier of the network response to search. </summary>
            public string requestId;
            /// <summary> [Require] String to search for. </summary>
            public string query;
            /// <summary> [Optional] If true, search is case sensitive. </summary>
            public bool caseSensitive;
            /// <summary> [Optional] If true, treats string parameter as regex. </summary>
            public bool isRegex;
        }
        public class SetBlockedURLsParameters
        {
            
            /// <summary> [Require] URL patterns to block. Wildcards ('*') are allowed. </summary>
            public object[] urls;
        }
        public class SetBypassServiceWorkerParameters
        {
            
            /// <summary> [Require] Bypass service worker and load from network. </summary>
            public bool bypass;
        }
        public class SetCacheDisabledParameters
        {
            
            /// <summary> [Require] Cache disabled state. </summary>
            public bool cacheDisabled;
        }
        public class SetCookieParameters
        {
            
            /// <summary> [Require] Cookie name. </summary>
            public string name;
            /// <summary> [Require] Cookie value. </summary>
            public string value;
            /// <summary> [Optional] The request-URI to associate with the setting of the cookie. This value can affect thedefault domain, path, source port, and source scheme values of the created cookie. </summary>
            public string url;
            /// <summary> [Optional] Cookie domain. </summary>
            public string domain;
            /// <summary> [Optional] Cookie path. </summary>
            public string path;
            /// <summary> [Optional] True if cookie is secure. </summary>
            public bool secure;
            /// <summary> [Optional] True if cookie is http-only. </summary>
            public bool httpOnly;
            /// <summary> [Optional] Cookie SameSite type. </summary>
            public string sameSite;
            /// <summary> [Optional] Cookie expiration date, session cookie if not set </summary>
            public double expires;
            /// <summary> [Optional] Cookie Priority type. </summary>
            public string priority;
            /// <summary> [Optional] True if cookie is SameParty. </summary>
            public bool sameParty;
            /// <summary> [Optional] Cookie source scheme type. </summary>
            public string sourceScheme;
            /// <summary> [Optional] Cookie source port. Valid values are {-1, [1, 65535]}, -1 indicates an unspecified port.An unspecified port value allows protocol clients to emulate legacy cookie scope for the port.This is a temporary ability and it will be removed in the future. </summary>
            public int sourcePort;
            /// <summary> [Optional] Cookie partition key. The site of the top-level URL the browser was visiting at the startof the request to the endpoint that set the cookie.If not set, the cookie will be set as not partitioned. </summary>
            public string partitionKey;
        }
        public class SetCookiesParameters
        {
            
            /// <summary> [Require] Cookies to be set. </summary>
            public object[] cookies;
        }
        public class SetExtraHTTPHeadersParameters
        {
            
            /// <summary> [Require] Map with extra HTTP headers. </summary>
            public Network.HeadersType headers;
        }
        public class SetAttachDebugStackParameters
        {
            
            /// <summary> [Require] Whether to attach a page script stack for debugging purpose. </summary>
            public bool enabled;
        }
        public class SetRequestInterceptionParameters
        {
            
            /// <summary> [Require] Requests matching any of these patterns will be forwarded and wait for the correspondingcontinueInterceptedRequest call. </summary>
            public object[] patterns;
        }
        public class SetUserAgentOverrideParameters
        {
            
            /// <summary> [Require] User agent to use. </summary>
            public string userAgent;
            /// <summary> [Optional] Browser langugage to emulate. </summary>
            public string acceptLanguage;
            /// <summary> [Optional] The platform navigator.platform should return. </summary>
            public string platform;
            /// <summary> [Optional] To be sent in Sec-CH-UA-* headers and returned in navigator.userAgentData </summary>
            public Emulation.UserAgentMetadataType userAgentMetadata;
        }
        public class GetSecurityIsolationStatusParameters
        {
            
            /// <summary> [Optional] If no frameId is provided, the status of the target is provided. </summary>
            public string frameId;
        }
        public class EnableReportingApiParameters
        {
            
            /// <summary> [Require] Whether to enable or disable events for the Reporting API </summary>
            public bool enable;
        }
        public class LoadNetworkResourceParameters
        {
            
            /// <summary> [Optional] Frame id to get the resource for. Mandatory for frame targets, andshould be omitted for worker targets. </summary>
            public string frameId;
            /// <summary> [Require] URL of the resource to get content for. </summary>
            public string url;
            /// <summary> [Require] Options for the request. </summary>
            public Network.LoadNetworkResourceOptionsType options;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class CanClearBrowserCacheReturn
        {
            
            /// <summary> True if browser cache can be cleared. </summary>
            public bool result;
        }
        public class CanClearBrowserCookiesReturn
        {
            
            /// <summary> True if browser cookies can be cleared. </summary>
            public bool result;
        }
        public class CanEmulateNetworkConditionsReturn
        {
            
            /// <summary> True if emulation of network conditions is supported. </summary>
            public bool result;
        }
        public class GetAllCookiesReturn
        {
            
            /// <summary> Array of cookie objects. </summary>
            public object[] cookies;
        }
        public class GetCertificateReturn
        {
            
            /// <summary>  </summary>
            public object[] tableNames;
        }
        public class GetCookiesReturn
        {
            
            /// <summary> Array of cookie objects. </summary>
            public object[] cookies;
        }
        public class GetResponseBodyReturn
        {
            
            /// <summary> Response body. </summary>
            public string body;
            /// <summary> True, if content was sent as base64. </summary>
            public bool base64Encoded;
        }
        public class GetRequestPostDataReturn
        {
            
            /// <summary> Request body string, omitting files from multipart requests </summary>
            public string postData;
        }
        public class GetResponseBodyForInterceptionReturn
        {
            
            /// <summary> Response body. </summary>
            public string body;
            /// <summary> True, if content was sent as base64. </summary>
            public bool base64Encoded;
        }
        public class TakeResponseBodyForInterceptionAsStreamReturn
        {
            
            /// <summary>  </summary>
            public string stream;
        }
        public class SearchInResponseBodyReturn
        {
            
            /// <summary> List of search matches. </summary>
            public object[] result;
        }
        public class SetCookieReturn
        {
            
            /// <summary> Always set to true. If an error occurs, the response indicates protocol error. </summary>
            public bool success;
        }
        public class GetSecurityIsolationStatusReturn
        {
            
            /// <summary>  </summary>
            public Network.SecurityIsolationStatusType status;
        }
        public class LoadNetworkResourceReturn
        {
            
            /// <summary>  </summary>
            public Network.LoadNetworkResourcePageResultType resource;
        }
    }
    
    public class Overlay : DomainBase
    {
        public Overlay(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Fired when the node should be inspected. This happens after call to `setInspectMode` or whenuser manually inspects an element. </summary>
        /// <returns> remove handler </returns>
        public Action OnInspectNodeRequested(Action<OnInspectNodeRequestedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnInspectNodeRequestedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Overlay.inspectNodeRequested" : $"Overlay.inspectNodeRequested.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when the node should be highlighted. This happens after call to `setInspectMode`. </summary>
        /// <returns> remove handler </returns>
        public Action OnNodeHighlightRequested(Action<OnNodeHighlightRequestedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnNodeHighlightRequestedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Overlay.nodeHighlightRequested" : $"Overlay.nodeHighlightRequested.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when user asks to capture screenshot of some area on the page. </summary>
        /// <returns> remove handler </returns>
        public Action OnScreenshotRequested(Action<OnScreenshotRequestedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnScreenshotRequestedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Overlay.screenshotRequested" : $"Overlay.screenshotRequested.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when user cancels the inspect mode. </summary>
        /// <returns> remove handler </returns>
        public Action OnInspectModeCanceled(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "Overlay.inspectModeCanceled" : $"Overlay.inspectModeCanceled.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Disables domain notifications. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables domain notifications. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// For testing. 
        /// </summary>
        public async Task<GetHighlightObjectForTestReturn> GetHighlightObjectForTest(GetHighlightObjectForTestParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.getHighlightObjectForTest", parameters, sessionId);
            return Convert<GetHighlightObjectForTestReturn>(___r);
        }
        /// <summary> 
        /// For Persistent Grid testing. 
        /// </summary>
        public async Task<GetGridHighlightObjectsForTestReturn> GetGridHighlightObjectsForTest(GetGridHighlightObjectsForTestParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.getGridHighlightObjectsForTest", parameters, sessionId);
            return Convert<GetGridHighlightObjectsForTestReturn>(___r);
        }
        /// <summary> 
        /// For Source Order Viewer testing. 
        /// </summary>
        public async Task<GetSourceOrderHighlightObjectForTestReturn> GetSourceOrderHighlightObjectForTest(GetSourceOrderHighlightObjectForTestParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.getSourceOrderHighlightObjectForTest", parameters, sessionId);
            return Convert<GetSourceOrderHighlightObjectForTestReturn>(___r);
        }
        /// <summary> 
        /// Hides any highlight. 
        /// </summary>
        public async Task HideHighlight(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.hideHighlight", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Highlights owner element of the frame with given id.Deprecated: Doesn't work reliablity and cannot be fixed due to processseparatation (the owner node might be in a different process). Determinethe owner node in the client and use highlightNode. 
        /// </summary>
        public async Task HighlightFrame(HighlightFrameParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.highlightFrame", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Highlights DOM node with given id or with the given JavaScript object wrapper. Either nodeId orobjectId must be specified. 
        /// </summary>
        public async Task HighlightNode(HighlightNodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.highlightNode", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Highlights given quad. Coordinates are absolute with respect to the main frame viewport. 
        /// </summary>
        public async Task HighlightQuad(HighlightQuadParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.highlightQuad", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Highlights given rectangle. Coordinates are absolute with respect to the main frame viewport. 
        /// </summary>
        public async Task HighlightRect(HighlightRectParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.highlightRect", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Highlights the source order of the children of the DOM node with given id or with the givenJavaScript object wrapper. Either nodeId or objectId must be specified. 
        /// </summary>
        public async Task HighlightSourceOrder(HighlightSourceOrderParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.highlightSourceOrder", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Enters the 'inspect' mode. In this mode, elements that user is hovering over are highlighted.Backend then generates 'inspectNodeRequested' event upon element selection. 
        /// </summary>
        public async Task SetInspectMode(SetInspectModeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setInspectMode", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Highlights owner element of all frames detected to be ads. 
        /// </summary>
        public async Task SetShowAdHighlights(SetShowAdHighlightsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowAdHighlights", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetPausedInDebuggerMessage(SetPausedInDebuggerMessageParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setPausedInDebuggerMessage", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests that backend shows debug borders on layers 
        /// </summary>
        public async Task SetShowDebugBorders(SetShowDebugBordersParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowDebugBorders", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests that backend shows the FPS counter 
        /// </summary>
        public async Task SetShowFPSCounter(SetShowFPSCounterParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowFPSCounter", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Highlight multiple elements with the CSS Grid overlay. 
        /// </summary>
        public async Task SetShowGridOverlays(SetShowGridOverlaysParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowGridOverlays", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetShowFlexOverlays(SetShowFlexOverlaysParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowFlexOverlays", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetShowScrollSnapOverlays(SetShowScrollSnapOverlaysParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowScrollSnapOverlays", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetShowContainerQueryOverlays(SetShowContainerQueryOverlaysParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowContainerQueryOverlays", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests that backend shows paint rectangles 
        /// </summary>
        public async Task SetShowPaintRects(SetShowPaintRectsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowPaintRects", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests that backend shows layout shift regions 
        /// </summary>
        public async Task SetShowLayoutShiftRegions(SetShowLayoutShiftRegionsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowLayoutShiftRegions", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests that backend shows scroll bottleneck rects 
        /// </summary>
        public async Task SetShowScrollBottleneckRects(SetShowScrollBottleneckRectsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowScrollBottleneckRects", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Deprecated, no longer has any effect. 
        /// </summary>
        public async Task SetShowHitTestBorders(SetShowHitTestBordersParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowHitTestBorders", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Request that backend shows an overlay with web vital metrics. 
        /// </summary>
        public async Task SetShowWebVitals(SetShowWebVitalsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowWebVitals", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Paints viewport size upon main frame resize. 
        /// </summary>
        public async Task SetShowViewportSizeOnResize(SetShowViewportSizeOnResizeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowViewportSizeOnResize", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Add a dual screen device hinge 
        /// </summary>
        public async Task SetShowHinge(SetShowHingeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowHinge", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Show elements in isolation mode with overlays. 
        /// </summary>
        public async Task SetShowIsolatedElements(SetShowIsolatedElementsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Overlay.setShowIsolatedElements", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class SourceOrderConfigType
        {
            
            /// <summary> the color to outline the givent element in. </summary>
            public DOM.RGBAType parentOutlineColor;
            /// <summary> the color to outline the child elements in. </summary>
            public DOM.RGBAType childOutlineColor;
        }
        public class GridHighlightConfigType
        {
            
            /// <summary> Whether the extension lines from grid cells to the rulers should be shown (default: false). </summary>
            public bool showGridExtensionLines;
            /// <summary> Show Positive line number labels (default: false). </summary>
            public bool showPositiveLineNumbers;
            /// <summary> Show Negative line number labels (default: false). </summary>
            public bool showNegativeLineNumbers;
            /// <summary> Show area name labels (default: false). </summary>
            public bool showAreaNames;
            /// <summary> Show line name labels (default: false). </summary>
            public bool showLineNames;
            /// <summary> Show track size labels (default: false). </summary>
            public bool showTrackSizes;
            /// <summary> The grid container border highlight color (default: transparent). </summary>
            public DOM.RGBAType gridBorderColor;
            /// <summary> The cell border color (default: transparent). Deprecated, please use rowLineColor and columnLineColor instead. </summary>
            public DOM.RGBAType cellBorderColor;
            /// <summary> The row line color (default: transparent). </summary>
            public DOM.RGBAType rowLineColor;
            /// <summary> The column line color (default: transparent). </summary>
            public DOM.RGBAType columnLineColor;
            /// <summary> Whether the grid border is dashed (default: false). </summary>
            public bool gridBorderDash;
            /// <summary> Whether the cell border is dashed (default: false). Deprecated, please us rowLineDash and columnLineDash instead. </summary>
            public bool cellBorderDash;
            /// <summary> Whether row lines are dashed (default: false). </summary>
            public bool rowLineDash;
            /// <summary> Whether column lines are dashed (default: false). </summary>
            public bool columnLineDash;
            /// <summary> The row gap highlight fill color (default: transparent). </summary>
            public DOM.RGBAType rowGapColor;
            /// <summary> The row gap hatching fill color (default: transparent). </summary>
            public DOM.RGBAType rowHatchColor;
            /// <summary> The column gap highlight fill color (default: transparent). </summary>
            public DOM.RGBAType columnGapColor;
            /// <summary> The column gap hatching fill color (default: transparent). </summary>
            public DOM.RGBAType columnHatchColor;
            /// <summary> The named grid areas border color (Default: transparent). </summary>
            public DOM.RGBAType areaBorderColor;
            /// <summary> The grid container background color (Default: transparent). </summary>
            public DOM.RGBAType gridBackgroundColor;
        }
        public class FlexContainerHighlightConfigType
        {
            
            /// <summary> The style of the container border </summary>
            public Overlay.LineStyleType containerBorder;
            /// <summary> The style of the separator between lines </summary>
            public Overlay.LineStyleType lineSeparator;
            /// <summary> The style of the separator between items </summary>
            public Overlay.LineStyleType itemSeparator;
            /// <summary> Style of content-distribution space on the main axis (justify-content). </summary>
            public Overlay.BoxStyleType mainDistributedSpace;
            /// <summary> Style of content-distribution space on the cross axis (align-content). </summary>
            public Overlay.BoxStyleType crossDistributedSpace;
            /// <summary> Style of empty space caused by row gaps (gap/row-gap). </summary>
            public Overlay.BoxStyleType rowGapSpace;
            /// <summary> Style of empty space caused by columns gaps (gap/column-gap). </summary>
            public Overlay.BoxStyleType columnGapSpace;
            /// <summary> Style of the self-alignment line (align-items). </summary>
            public Overlay.LineStyleType crossAlignment;
        }
        public class FlexItemHighlightConfigType
        {
            
            /// <summary> Style of the box representing the item's base size </summary>
            public Overlay.BoxStyleType baseSizeBox;
            /// <summary> Style of the border around the box representing the item's base size </summary>
            public Overlay.LineStyleType baseSizeBorder;
            /// <summary> Style of the arrow representing if the item grew or shrank </summary>
            public Overlay.LineStyleType flexibilityArrow;
        }
        public class LineStyleType
        {
            
            /// <summary> The color of the line (default: transparent) </summary>
            public DOM.RGBAType color;
            /// <summary> The line pattern (default: solid) </summary>
            public string pattern;
        }
        public class BoxStyleType
        {
            
            /// <summary> The background color for the box (default: transparent) </summary>
            public DOM.RGBAType fillColor;
            /// <summary> The hatching color for the box (default: transparent) </summary>
            public DOM.RGBAType hatchColor;
        }
        public class HighlightConfigType
        {
            
            /// <summary> Whether the node info tooltip should be shown (default: false). </summary>
            public bool showInfo;
            /// <summary> Whether the node styles in the tooltip (default: false). </summary>
            public bool showStyles;
            /// <summary> Whether the rulers should be shown (default: false). </summary>
            public bool showRulers;
            /// <summary> Whether the a11y info should be shown (default: true). </summary>
            public bool showAccessibilityInfo;
            /// <summary> Whether the extension lines from node to the rulers should be shown (default: false). </summary>
            public bool showExtensionLines;
            /// <summary> The content box highlight fill color (default: transparent). </summary>
            public DOM.RGBAType contentColor;
            /// <summary> The padding highlight fill color (default: transparent). </summary>
            public DOM.RGBAType paddingColor;
            /// <summary> The border highlight fill color (default: transparent). </summary>
            public DOM.RGBAType borderColor;
            /// <summary> The margin highlight fill color (default: transparent). </summary>
            public DOM.RGBAType marginColor;
            /// <summary> The event target element highlight fill color (default: transparent). </summary>
            public DOM.RGBAType eventTargetColor;
            /// <summary> The shape outside fill color (default: transparent). </summary>
            public DOM.RGBAType shapeColor;
            /// <summary> The shape margin fill color (default: transparent). </summary>
            public DOM.RGBAType shapeMarginColor;
            /// <summary> The grid layout color (default: transparent). </summary>
            public DOM.RGBAType cssGridColor;
            /// <summary> The color format used to format color styles (default: hex). </summary>
            public string colorFormat;
            /// <summary> The grid layout highlight configuration (default: all transparent). </summary>
            public Overlay.GridHighlightConfigType gridHighlightConfig;
            /// <summary> The flex container highlight configuration (default: all transparent). </summary>
            public Overlay.FlexContainerHighlightConfigType flexContainerHighlightConfig;
            /// <summary> The flex item highlight configuration (default: all transparent). </summary>
            public Overlay.FlexItemHighlightConfigType flexItemHighlightConfig;
            /// <summary> The contrast algorithm to use for the contrast ratio (default: aa). </summary>
            public string contrastAlgorithm;
            /// <summary> The container query container highlight configuration (default: all transparent). </summary>
            public Overlay.ContainerQueryContainerHighlightConfigType containerQueryContainerHighlightConfig;
        }
        public class GridNodeHighlightConfigType
        {
            
            /// <summary> A descriptor for the highlight appearance. </summary>
            public Overlay.GridHighlightConfigType gridHighlightConfig;
            /// <summary> Identifier of the node to highlight. </summary>
            public int nodeId;
        }
        public class FlexNodeHighlightConfigType
        {
            
            /// <summary> A descriptor for the highlight appearance of flex containers. </summary>
            public Overlay.FlexContainerHighlightConfigType flexContainerHighlightConfig;
            /// <summary> Identifier of the node to highlight. </summary>
            public int nodeId;
        }
        public class ScrollSnapContainerHighlightConfigType
        {
            
            /// <summary> The style of the snapport border (default: transparent) </summary>
            public Overlay.LineStyleType snapportBorder;
            /// <summary> The style of the snap area border (default: transparent) </summary>
            public Overlay.LineStyleType snapAreaBorder;
            /// <summary> The margin highlight fill color (default: transparent). </summary>
            public DOM.RGBAType scrollMarginColor;
            /// <summary> The padding highlight fill color (default: transparent). </summary>
            public DOM.RGBAType scrollPaddingColor;
        }
        public class ScrollSnapHighlightConfigType
        {
            
            /// <summary> A descriptor for the highlight appearance of scroll snap containers. </summary>
            public Overlay.ScrollSnapContainerHighlightConfigType scrollSnapContainerHighlightConfig;
            /// <summary> Identifier of the node to highlight. </summary>
            public int nodeId;
        }
        public class HingeConfigType
        {
            
            /// <summary> A rectangle represent hinge </summary>
            public DOM.RectType rect;
            /// <summary> The content box highlight fill color (default: a dark color). </summary>
            public DOM.RGBAType contentColor;
            /// <summary> The content box highlight outline color (default: transparent). </summary>
            public DOM.RGBAType outlineColor;
        }
        public class ContainerQueryHighlightConfigType
        {
            
            /// <summary> A descriptor for the highlight appearance of container query containers. </summary>
            public Overlay.ContainerQueryContainerHighlightConfigType containerQueryContainerHighlightConfig;
            /// <summary> Identifier of the container node to highlight. </summary>
            public int nodeId;
        }
        public class ContainerQueryContainerHighlightConfigType
        {
            
            /// <summary> The style of the container border. </summary>
            public Overlay.LineStyleType containerBorder;
            /// <summary> The style of the descendants' borders. </summary>
            public Overlay.LineStyleType descendantBorder;
        }
        public class IsolatedElementHighlightConfigType
        {
            
            /// <summary> A descriptor for the highlight appearance of an element in isolation mode. </summary>
            public Overlay.IsolationModeHighlightConfigType isolationModeHighlightConfig;
            /// <summary> Identifier of the isolated element to highlight. </summary>
            public int nodeId;
        }
        public class IsolationModeHighlightConfigType
        {
            
            /// <summary> The fill color of the resizers (default: transparent). </summary>
            public DOM.RGBAType resizerColor;
            /// <summary> The fill color for resizer handles (default: transparent). </summary>
            public DOM.RGBAType resizerHandleColor;
            /// <summary> The fill color for the mask covering non-isolated elements (default: transparent). </summary>
            public DOM.RGBAType maskColor;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnInspectNodeRequestedParameters
        {
            
            /// <summary> [Require] Id of the node to inspect. </summary>
            public int backendNodeId;
        }
        public class OnNodeHighlightRequestedParameters
        {
            
            /// <summary> [Require]  </summary>
            public int nodeId;
        }
        public class OnScreenshotRequestedParameters
        {
            
            /// <summary> [Require] Viewport to capture, in device independent pixels (dip). </summary>
            public Page.ViewportType viewport;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class GetHighlightObjectForTestParameters
        {
            
            /// <summary> [Require] Id of the node to get highlight object for. </summary>
            public int nodeId;
            /// <summary> [Optional] Whether to include distance info. </summary>
            public bool includeDistance;
            /// <summary> [Optional] Whether to include style info. </summary>
            public bool includeStyle;
            /// <summary> [Optional] The color format to get config with (default: hex). </summary>
            public string colorFormat;
            /// <summary> [Optional] Whether to show accessibility info (default: true). </summary>
            public bool showAccessibilityInfo;
        }
        public class GetGridHighlightObjectsForTestParameters
        {
            
            /// <summary> [Require] Ids of the node to get highlight object for. </summary>
            public object[] nodeIds;
        }
        public class GetSourceOrderHighlightObjectForTestParameters
        {
            
            /// <summary> [Require] Id of the node to highlight. </summary>
            public int nodeId;
        }
        public class HighlightFrameParameters
        {
            
            /// <summary> [Require] Identifier of the frame to highlight. </summary>
            public string frameId;
            /// <summary> [Optional] The content box highlight fill color (default: transparent). </summary>
            public DOM.RGBAType contentColor;
            /// <summary> [Optional] The content box highlight outline color (default: transparent). </summary>
            public DOM.RGBAType contentOutlineColor;
        }
        public class HighlightNodeParameters
        {
            
            /// <summary> [Require] A descriptor for the highlight appearance. </summary>
            public Overlay.HighlightConfigType highlightConfig;
            /// <summary> [Optional] Identifier of the node to highlight. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node to highlight. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node to be highlighted. </summary>
            public string objectId;
            /// <summary> [Optional] Selectors to highlight relevant nodes. </summary>
            public string selector;
        }
        public class HighlightQuadParameters
        {
            
            /// <summary> [Require] Quad to highlight </summary>
            public object[] quad;
            /// <summary> [Optional] The highlight fill color (default: transparent). </summary>
            public DOM.RGBAType color;
            /// <summary> [Optional] The highlight outline color (default: transparent). </summary>
            public DOM.RGBAType outlineColor;
        }
        public class HighlightRectParameters
        {
            
            /// <summary> [Require] X coordinate </summary>
            public int x;
            /// <summary> [Require] Y coordinate </summary>
            public int y;
            /// <summary> [Require] Rectangle width </summary>
            public int width;
            /// <summary> [Require] Rectangle height </summary>
            public int height;
            /// <summary> [Optional] The highlight fill color (default: transparent). </summary>
            public DOM.RGBAType color;
            /// <summary> [Optional] The highlight outline color (default: transparent). </summary>
            public DOM.RGBAType outlineColor;
        }
        public class HighlightSourceOrderParameters
        {
            
            /// <summary> [Require] A descriptor for the appearance of the overlay drawing. </summary>
            public Overlay.SourceOrderConfigType sourceOrderConfig;
            /// <summary> [Optional] Identifier of the node to highlight. </summary>
            public int nodeId;
            /// <summary> [Optional] Identifier of the backend node to highlight. </summary>
            public int backendNodeId;
            /// <summary> [Optional] JavaScript object id of the node to be highlighted. </summary>
            public string objectId;
        }
        public class SetInspectModeParameters
        {
            
            /// <summary> [Require] Set an inspection mode. </summary>
            public string mode;
            /// <summary> [Optional] A descriptor for the highlight appearance of hovered-over nodes. May be omitted if `enabled== false`. </summary>
            public Overlay.HighlightConfigType highlightConfig;
        }
        public class SetShowAdHighlightsParameters
        {
            
            /// <summary> [Require] True for showing ad highlights </summary>
            public bool show;
        }
        public class SetPausedInDebuggerMessageParameters
        {
            
            /// <summary> [Optional] The message to display, also triggers resume and step over controls. </summary>
            public string message;
        }
        public class SetShowDebugBordersParameters
        {
            
            /// <summary> [Require] True for showing debug borders </summary>
            public bool show;
        }
        public class SetShowFPSCounterParameters
        {
            
            /// <summary> [Require] True for showing the FPS counter </summary>
            public bool show;
        }
        public class SetShowGridOverlaysParameters
        {
            
            /// <summary> [Require] An array of node identifiers and descriptors for the highlight appearance. </summary>
            public object[] gridNodeHighlightConfigs;
        }
        public class SetShowFlexOverlaysParameters
        {
            
            /// <summary> [Require] An array of node identifiers and descriptors for the highlight appearance. </summary>
            public object[] flexNodeHighlightConfigs;
        }
        public class SetShowScrollSnapOverlaysParameters
        {
            
            /// <summary> [Require] An array of node identifiers and descriptors for the highlight appearance. </summary>
            public object[] scrollSnapHighlightConfigs;
        }
        public class SetShowContainerQueryOverlaysParameters
        {
            
            /// <summary> [Require] An array of node identifiers and descriptors for the highlight appearance. </summary>
            public object[] containerQueryHighlightConfigs;
        }
        public class SetShowPaintRectsParameters
        {
            
            /// <summary> [Require] True for showing paint rectangles </summary>
            public bool result;
        }
        public class SetShowLayoutShiftRegionsParameters
        {
            
            /// <summary> [Require] True for showing layout shift regions </summary>
            public bool result;
        }
        public class SetShowScrollBottleneckRectsParameters
        {
            
            /// <summary> [Require] True for showing scroll bottleneck rects </summary>
            public bool show;
        }
        public class SetShowHitTestBordersParameters
        {
            
            /// <summary> [Require] True for showing hit-test borders </summary>
            public bool show;
        }
        public class SetShowWebVitalsParameters
        {
            
            /// <summary> [Require]  </summary>
            public bool show;
        }
        public class SetShowViewportSizeOnResizeParameters
        {
            
            /// <summary> [Require] Whether to paint size or not. </summary>
            public bool show;
        }
        public class SetShowHingeParameters
        {
            
            /// <summary> [Optional] hinge data, null means hideHinge </summary>
            public Overlay.HingeConfigType hingeConfig;
        }
        public class SetShowIsolatedElementsParameters
        {
            
            /// <summary> [Require] An array of node identifiers and descriptors for the highlight appearance. </summary>
            public object[] isolatedElementHighlightConfigs;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetHighlightObjectForTestReturn
        {
            
            /// <summary> Highlight data for the node. </summary>
            public object highlight;
        }
        public class GetGridHighlightObjectsForTestReturn
        {
            
            /// <summary> Grid Highlight data for the node ids provided. </summary>
            public object highlights;
        }
        public class GetSourceOrderHighlightObjectForTestReturn
        {
            
            /// <summary> Source order highlight data for the node id provided. </summary>
            public object highlight;
        }
    }
    
    public class Page : DomainBase
    {
        public Page(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnDomContentEventFired(Action<OnDomContentEventFiredParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDomContentEventFiredParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.domContentEventFired" : $"Page.domContentEventFired.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Emitted only when `page.interceptFileChooser` is enabled. </summary>
        /// <returns> remove handler </returns>
        public Action OnFileChooserOpened(Action<OnFileChooserOpenedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFileChooserOpenedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.fileChooserOpened" : $"Page.fileChooserOpened.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when frame has been attached to its parent. </summary>
        /// <returns> remove handler </returns>
        public Action OnFrameAttached(Action<OnFrameAttachedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFrameAttachedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.frameAttached" : $"Page.frameAttached.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when frame no longer has a scheduled navigation. </summary>
        /// <returns> remove handler </returns>
        public Action OnFrameClearedScheduledNavigation(Action<OnFrameClearedScheduledNavigationParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFrameClearedScheduledNavigationParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.frameClearedScheduledNavigation" : $"Page.frameClearedScheduledNavigation.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when frame has been detached from its parent. </summary>
        /// <returns> remove handler </returns>
        public Action OnFrameDetached(Action<OnFrameDetachedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFrameDetachedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.frameDetached" : $"Page.frameDetached.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired once navigation of the frame has completed. Frame is now associated with the new loader. </summary>
        /// <returns> remove handler </returns>
        public Action OnFrameNavigated(Action<OnFrameNavigatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFrameNavigatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.frameNavigated" : $"Page.frameNavigated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when opening document to write to. </summary>
        /// <returns> remove handler </returns>
        public Action OnDocumentOpened(Action<OnDocumentOpenedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDocumentOpenedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.documentOpened" : $"Page.documentOpened.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnFrameResized(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.frameResized" : $"Page.frameResized.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        
        /// <summary> Fired when a renderer-initiated navigation is requested.Navigation may still be cancelled after the event is issued. </summary>
        /// <returns> remove handler </returns>
        public Action OnFrameRequestedNavigation(Action<OnFrameRequestedNavigationParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFrameRequestedNavigationParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.frameRequestedNavigation" : $"Page.frameRequestedNavigation.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when frame schedules a potential navigation. </summary>
        /// <returns> remove handler </returns>
        public Action OnFrameScheduledNavigation(Action<OnFrameScheduledNavigationParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFrameScheduledNavigationParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.frameScheduledNavigation" : $"Page.frameScheduledNavigation.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when frame has started loading. </summary>
        /// <returns> remove handler </returns>
        public Action OnFrameStartedLoading(Action<OnFrameStartedLoadingParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFrameStartedLoadingParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.frameStartedLoading" : $"Page.frameStartedLoading.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when frame has stopped loading. </summary>
        /// <returns> remove handler </returns>
        public Action OnFrameStoppedLoading(Action<OnFrameStoppedLoadingParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnFrameStoppedLoadingParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.frameStoppedLoading" : $"Page.frameStoppedLoading.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when page is about to start a download.Deprecated. Use Browser.downloadWillBegin instead. </summary>
        /// <returns> remove handler </returns>
        public Action OnDownloadWillBegin(Action<OnDownloadWillBeginParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDownloadWillBeginParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.downloadWillBegin" : $"Page.downloadWillBegin.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when download makes progress. Last call has |done| == true.Deprecated. Use Browser.downloadProgress instead. </summary>
        /// <returns> remove handler </returns>
        public Action OnDownloadProgress(Action<OnDownloadProgressParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDownloadProgressParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.downloadProgress" : $"Page.downloadProgress.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when interstitial page was hidden </summary>
        /// <returns> remove handler </returns>
        public Action OnInterstitialHidden(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.interstitialHidden" : $"Page.interstitialHidden.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        
        /// <summary> Fired when interstitial page was shown </summary>
        /// <returns> remove handler </returns>
        public Action OnInterstitialShown(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.interstitialShown" : $"Page.interstitialShown.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        
        /// <summary> Fired when a JavaScript initiated dialog (alert, confirm, prompt, or onbeforeunload) has beenclosed. </summary>
        /// <returns> remove handler </returns>
        public Action OnJavascriptDialogClosed(Action<OnJavascriptDialogClosedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnJavascriptDialogClosedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.javascriptDialogClosed" : $"Page.javascriptDialogClosed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when a JavaScript initiated dialog (alert, confirm, prompt, or onbeforeunload) is about toopen. </summary>
        /// <returns> remove handler </returns>
        public Action OnJavascriptDialogOpening(Action<OnJavascriptDialogOpeningParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnJavascriptDialogOpeningParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.javascriptDialogOpening" : $"Page.javascriptDialogOpening.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired for top level page lifecycle events such as navigation, load, paint, etc. </summary>
        /// <returns> remove handler </returns>
        public Action OnLifecycleEvent(Action<OnLifecycleEventParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnLifecycleEventParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.lifecycleEvent" : $"Page.lifecycleEvent.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired for failed bfcache history navigations if BackForwardCache feature is enabled. Donot assume any ordering with the Page.frameNavigated event. This event is fired only formain-frame history navigation where the document changes (non-same-document navigations),when bfcache navigation fails. </summary>
        /// <returns> remove handler </returns>
        public Action OnBackForwardCacheNotUsed(Action<OnBackForwardCacheNotUsedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnBackForwardCacheNotUsedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.backForwardCacheNotUsed" : $"Page.backForwardCacheNotUsed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnLoadEventFired(Action<OnLoadEventFiredParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnLoadEventFiredParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.loadEventFired" : $"Page.loadEventFired.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when same-document navigation happens, e.g. due to history API usage or anchor navigation. </summary>
        /// <returns> remove handler </returns>
        public Action OnNavigatedWithinDocument(Action<OnNavigatedWithinDocumentParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnNavigatedWithinDocumentParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.navigatedWithinDocument" : $"Page.navigatedWithinDocument.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Compressed image data requested by the `startScreencast`. </summary>
        /// <returns> remove handler </returns>
        public Action OnScreencastFrame(Action<OnScreencastFrameParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnScreencastFrameParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.screencastFrame" : $"Page.screencastFrame.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when the page with currently enabled screencast was shown or hidden `. </summary>
        /// <returns> remove handler </returns>
        public Action OnScreencastVisibilityChanged(Action<OnScreencastVisibilityChangedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnScreencastVisibilityChangedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.screencastVisibilityChanged" : $"Page.screencastVisibilityChanged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when a new window is going to be opened, via window.open(), link click, form submission,etc. </summary>
        /// <returns> remove handler </returns>
        public Action OnWindowOpen(Action<OnWindowOpenParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWindowOpenParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.windowOpen" : $"Page.windowOpen.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued for every compilation cache generated. Is only availableif Page.setGenerateCompilationCache is enabled. </summary>
        /// <returns> remove handler </returns>
        public Action OnCompilationCacheProduced(Action<OnCompilationCacheProducedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnCompilationCacheProducedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Page.compilationCacheProduced" : $"Page.compilationCacheProduced.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Deprecated, please use addScriptToEvaluateOnNewDocument instead. 
        /// </summary>
        public async Task<AddScriptToEvaluateOnLoadReturn> AddScriptToEvaluateOnLoad(AddScriptToEvaluateOnLoadParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.addScriptToEvaluateOnLoad", parameters, sessionId);
            return Convert<AddScriptToEvaluateOnLoadReturn>(___r);
        }
        /// <summary> 
        /// Evaluates given script in every frame upon creation (before loading frame's scripts). 
        /// </summary>
        public async Task<AddScriptToEvaluateOnNewDocumentReturn> AddScriptToEvaluateOnNewDocument(AddScriptToEvaluateOnNewDocumentParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.addScriptToEvaluateOnNewDocument", parameters, sessionId);
            return Convert<AddScriptToEvaluateOnNewDocumentReturn>(___r);
        }
        /// <summary> 
        /// Brings page to front (activates tab). 
        /// </summary>
        public async Task BringToFront(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.bringToFront", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Capture page screenshot. 
        /// </summary>
        public async Task<CaptureScreenshotReturn> CaptureScreenshot(CaptureScreenshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.captureScreenshot", parameters, sessionId);
            return Convert<CaptureScreenshotReturn>(___r);
        }
        /// <summary> 
        /// Returns a snapshot of the page as a string. For MHTML format, the serialization includesiframes, shadow DOM, external resources, and element-inline styles. 
        /// </summary>
        public async Task<CaptureSnapshotReturn> CaptureSnapshot(CaptureSnapshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.captureSnapshot", parameters, sessionId);
            return Convert<CaptureSnapshotReturn>(___r);
        }
        /// <summary> 
        /// Clears the overridden device metrics. 
        /// </summary>
        public async Task ClearDeviceMetricsOverride(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.clearDeviceMetricsOverride", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears the overridden Device Orientation. 
        /// </summary>
        public async Task ClearDeviceOrientationOverride(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.clearDeviceOrientationOverride", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears the overridden Geolocation Position and Error. 
        /// </summary>
        public async Task ClearGeolocationOverride(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.clearGeolocationOverride", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Creates an isolated world for the given frame. 
        /// </summary>
        public async Task<CreateIsolatedWorldReturn> CreateIsolatedWorld(CreateIsolatedWorldParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.createIsolatedWorld", parameters, sessionId);
            return Convert<CreateIsolatedWorldReturn>(___r);
        }
        /// <summary> 
        /// Deletes browser cookie with given name, domain and path. 
        /// </summary>
        public async Task DeleteCookie(DeleteCookieParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.deleteCookie", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables page domain notifications. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables page domain notifications. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetAppManifestReturn> GetAppManifest(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getAppManifest", null, sessionId);
            return Convert<GetAppManifestReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetInstallabilityErrorsReturn> GetInstallabilityErrors(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getInstallabilityErrors", null, sessionId);
            return Convert<GetInstallabilityErrorsReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetManifestIconsReturn> GetManifestIcons(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getManifestIcons", null, sessionId);
            return Convert<GetManifestIconsReturn>(___r);
        }
        /// <summary> 
        /// Returns the unique (PWA) app id.Only returns values if the feature flag 'WebAppEnableManifestId' is enabled 
        /// </summary>
        public async Task<GetAppIdReturn> GetAppId(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getAppId", null, sessionId);
            return Convert<GetAppIdReturn>(___r);
        }
        /// <summary> 
        /// Returns all browser cookies. Depending on the backend support, will return detailed cookieinformation in the `cookies` field. 
        /// </summary>
        public async Task<GetCookiesReturn> GetCookies(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getCookies", null, sessionId);
            return Convert<GetCookiesReturn>(___r);
        }
        /// <summary> 
        /// Returns present frame tree structure. 
        /// </summary>
        public async Task<GetFrameTreeReturn> GetFrameTree(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getFrameTree", null, sessionId);
            return Convert<GetFrameTreeReturn>(___r);
        }
        /// <summary> 
        /// Returns metrics relating to the layouting of the page, such as viewport bounds/scale. 
        /// </summary>
        public async Task<GetLayoutMetricsReturn> GetLayoutMetrics(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getLayoutMetrics", null, sessionId);
            return Convert<GetLayoutMetricsReturn>(___r);
        }
        /// <summary> 
        /// Returns navigation history for the current page. 
        /// </summary>
        public async Task<GetNavigationHistoryReturn> GetNavigationHistory(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getNavigationHistory", null, sessionId);
            return Convert<GetNavigationHistoryReturn>(___r);
        }
        /// <summary> 
        /// Resets navigation history for the current page. 
        /// </summary>
        public async Task ResetNavigationHistory(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.resetNavigationHistory", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns content of the given resource. 
        /// </summary>
        public async Task<GetResourceContentReturn> GetResourceContent(GetResourceContentParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getResourceContent", parameters, sessionId);
            return Convert<GetResourceContentReturn>(___r);
        }
        /// <summary> 
        /// Returns present frame / resource tree structure. 
        /// </summary>
        public async Task<GetResourceTreeReturn> GetResourceTree(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getResourceTree", null, sessionId);
            return Convert<GetResourceTreeReturn>(___r);
        }
        /// <summary> 
        /// Accepts or dismisses a JavaScript initiated dialog (alert, confirm, prompt, or onbeforeunload). 
        /// </summary>
        public async Task HandleJavaScriptDialog(HandleJavaScriptDialogParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.handleJavaScriptDialog", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Navigates current page to the given URL. 
        /// </summary>
        public async Task<NavigateReturn> Navigate(NavigateParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.navigate", parameters, sessionId);
            return Convert<NavigateReturn>(___r);
        }
        /// <summary> 
        /// Navigates current page to the given history entry. 
        /// </summary>
        public async Task NavigateToHistoryEntry(NavigateToHistoryEntryParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.navigateToHistoryEntry", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Print page as PDF. 
        /// </summary>
        public async Task<PrintToPDFReturn> PrintToPDF(PrintToPDFParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.printToPDF", parameters, sessionId);
            return Convert<PrintToPDFReturn>(___r);
        }
        /// <summary> 
        /// Reloads given page optionally ignoring the cache. 
        /// </summary>
        public async Task Reload(ReloadParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.reload", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Deprecated, please use removeScriptToEvaluateOnNewDocument instead. 
        /// </summary>
        public async Task RemoveScriptToEvaluateOnLoad(RemoveScriptToEvaluateOnLoadParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.removeScriptToEvaluateOnLoad", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Removes given script from the list. 
        /// </summary>
        public async Task RemoveScriptToEvaluateOnNewDocument(RemoveScriptToEvaluateOnNewDocumentParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.removeScriptToEvaluateOnNewDocument", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Acknowledges that a screencast frame has been received by the frontend. 
        /// </summary>
        public async Task ScreencastFrameAck(ScreencastFrameAckParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.screencastFrameAck", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Searches for given string in resource content. 
        /// </summary>
        public async Task<SearchInResourceReturn> SearchInResource(SearchInResourceParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.searchInResource", parameters, sessionId);
            return Convert<SearchInResourceReturn>(___r);
        }
        /// <summary> 
        /// Enable Chrome's experimental ad filter on all sites. 
        /// </summary>
        public async Task SetAdBlockingEnabled(SetAdBlockingEnabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setAdBlockingEnabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Enable page Content Security Policy by-passing. 
        /// </summary>
        public async Task SetBypassCSP(SetBypassCSPParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setBypassCSP", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Get Permissions Policy state on given frame. 
        /// </summary>
        public async Task<GetPermissionsPolicyStateReturn> GetPermissionsPolicyState(GetPermissionsPolicyStateParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getPermissionsPolicyState", parameters, sessionId);
            return Convert<GetPermissionsPolicyStateReturn>(___r);
        }
        /// <summary> 
        /// Get Origin Trials on given frame. 
        /// </summary>
        public async Task<GetOriginTrialsReturn> GetOriginTrials(GetOriginTrialsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.getOriginTrials", parameters, sessionId);
            return Convert<GetOriginTrialsReturn>(___r);
        }
        /// <summary> 
        /// Overrides the values of device screen dimensions (window.screen.width, window.screen.height,window.innerWidth, window.innerHeight, and "device-width"/"device-height"-related CSS mediaquery results). 
        /// </summary>
        public async Task SetDeviceMetricsOverride(SetDeviceMetricsOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setDeviceMetricsOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Overrides the Device Orientation. 
        /// </summary>
        public async Task SetDeviceOrientationOverride(SetDeviceOrientationOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setDeviceOrientationOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Set generic font families. 
        /// </summary>
        public async Task SetFontFamilies(SetFontFamiliesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setFontFamilies", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Set default font sizes. 
        /// </summary>
        public async Task SetFontSizes(SetFontSizesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setFontSizes", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets given markup as the document's HTML. 
        /// </summary>
        public async Task SetDocumentContent(SetDocumentContentParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setDocumentContent", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Set the behavior when downloading a file. 
        /// </summary>
        public async Task SetDownloadBehavior(SetDownloadBehaviorParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setDownloadBehavior", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Overrides the Geolocation Position or Error. Omitting any of the parameters emulates positionunavailable. 
        /// </summary>
        public async Task SetGeolocationOverride(SetGeolocationOverrideParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setGeolocationOverride", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Controls whether page will emit lifecycle events. 
        /// </summary>
        public async Task SetLifecycleEventsEnabled(SetLifecycleEventsEnabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setLifecycleEventsEnabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Toggles mouse event-based touch event emulation. 
        /// </summary>
        public async Task SetTouchEmulationEnabled(SetTouchEmulationEnabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setTouchEmulationEnabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Starts sending each frame using the `screencastFrame` event. 
        /// </summary>
        public async Task StartScreencast(StartScreencastParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.startScreencast", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Force the page stop all navigations and pending resource fetches. 
        /// </summary>
        public async Task StopLoading(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.stopLoading", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Crashes renderer on the IO thread, generates minidumps. 
        /// </summary>
        public async Task Crash(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.crash", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Tries to close page, running its beforeunload hooks, if any. 
        /// </summary>
        public async Task Close(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.close", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Tries to update the web lifecycle state of the page.It will transition the page to the given state according to:https://github.com/WICG/web-lifecycle/ 
        /// </summary>
        public async Task SetWebLifecycleState(SetWebLifecycleStateParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setWebLifecycleState", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Stops sending each frame in the `screencastFrame`. 
        /// </summary>
        public async Task StopScreencast(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.stopScreencast", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Requests backend to produce compilation cache for the specified scripts.`scripts` are appeneded to the list of scripts for which the cachewould be produced. The list may be reset during page navigation.When script with a matching URL is encountered, the cache is optionallyproduced upon backend discretion, based on internal heuristics.See also: `Page.compilationCacheProduced`. 
        /// </summary>
        public async Task ProduceCompilationCache(ProduceCompilationCacheParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.produceCompilationCache", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Seeds compilation cache for given url. Compilation cache does not survivecross-process navigation. 
        /// </summary>
        public async Task AddCompilationCache(AddCompilationCacheParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.addCompilationCache", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears seeded compilation cache. 
        /// </summary>
        public async Task ClearCompilationCache(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.clearCompilationCache", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets the Secure Payment Confirmation transaction mode.https://w3c.github.io/secure-payment-confirmation/#sctn-automation-set-spc-transaction-mode 
        /// </summary>
        public async Task SetSPCTransactionMode(SetSPCTransactionModeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setSPCTransactionMode", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Generates a report for testing. 
        /// </summary>
        public async Task GenerateTestReport(GenerateTestReportParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.generateTestReport", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Pauses page execution. Can be resumed using generic Runtime.runIfWaitingForDebugger. 
        /// </summary>
        public async Task WaitForDebugger(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.waitForDebugger", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Intercept file chooser requests and transfer control to protocol clients.When file chooser interception is enabled, native file chooser dialog is not shown.Instead, a protocol event `Page.fileChooserOpened` is emitted. 
        /// </summary>
        public async Task SetInterceptFileChooserDialog(SetInterceptFileChooserDialogParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Page.setInterceptFileChooserDialog", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class AdFrameStatusType
        {
            
            /// <summary>  </summary>
            public string adFrameType;
            /// <summary>  </summary>
            public object[] explanations;
        }
        public class PermissionsPolicyBlockLocatorType
        {
            
            /// <summary>  </summary>
            public string frameId;
            /// <summary>  </summary>
            public string blockReason;
        }
        public class PermissionsPolicyFeatureStateType
        {
            
            /// <summary>  </summary>
            public string feature;
            /// <summary>  </summary>
            public bool allowed;
            /// <summary>  </summary>
            public Page.PermissionsPolicyBlockLocatorType locator;
        }
        public class OriginTrialTokenType
        {
            
            /// <summary>  </summary>
            public string origin;
            /// <summary>  </summary>
            public bool matchSubDomains;
            /// <summary>  </summary>
            public string trialName;
            /// <summary>  </summary>
            public double expiryTime;
            /// <summary>  </summary>
            public bool isThirdParty;
            /// <summary>  </summary>
            public string usageRestriction;
        }
        public class OriginTrialTokenWithStatusType
        {
            
            /// <summary>  </summary>
            public string rawTokenText;
            /// <summary> `parsedToken` is present only when the token is extractable andparsable. </summary>
            public Page.OriginTrialTokenType parsedToken;
            /// <summary>  </summary>
            public string status;
        }
        public class OriginTrialType
        {
            
            /// <summary>  </summary>
            public string trialName;
            /// <summary>  </summary>
            public string status;
            /// <summary>  </summary>
            public object[] tokensWithStatus;
        }
        public class FrameType
        {
            
            /// <summary> Frame unique identifier. </summary>
            public string id;
            /// <summary> Parent frame identifier. </summary>
            public string parentId;
            /// <summary> Identifier of the loader associated with this frame. </summary>
            public string loaderId;
            /// <summary> Frame's name as specified in the tag. </summary>
            public string name;
            /// <summary> Frame document's URL without fragment. </summary>
            public string url;
            /// <summary> Frame document's URL fragment including the '#'. </summary>
            public string urlFragment;
            /// <summary> Frame document's registered domain, taking the public suffixes list into account.Extracted from the Frame's url.Example URLs: http://www.google.com/file.html -> "google.com"              http://a.b.co.uk/file.html      -> "b.co.uk" </summary>
            public string domainAndRegistry;
            /// <summary> Frame document's security origin. </summary>
            public string securityOrigin;
            /// <summary> Frame document's mimeType as determined by the browser. </summary>
            public string mimeType;
            /// <summary> If the frame failed to load, this contains the URL that could not be loaded. Note that unlike url above, this URL may contain a fragment. </summary>
            public string unreachableUrl;
            /// <summary> Indicates whether this frame was tagged as an ad and why. </summary>
            public Page.AdFrameStatusType adFrameStatus;
            /// <summary> Indicates whether the main document is a secure context and explains why that is the case. </summary>
            public string secureContextType;
            /// <summary> Indicates whether this is a cross origin isolated context. </summary>
            public string crossOriginIsolatedContextType;
            /// <summary> Indicated which gated APIs / features are available. </summary>
            public object[] gatedAPIFeatures;
        }
        public class FrameResourceType
        {
            
            /// <summary> Resource URL. </summary>
            public string url;
            /// <summary> Type of this resource. </summary>
            public string type;
            /// <summary> Resource mimeType as determined by the browser. </summary>
            public string mimeType;
            /// <summary> last-modified timestamp as reported by server. </summary>
            public double lastModified;
            /// <summary> Resource content size. </summary>
            public double contentSize;
            /// <summary> True if the resource failed to load. </summary>
            public bool failed;
            /// <summary> True if the resource was canceled during loading. </summary>
            public bool canceled;
        }
        public class FrameResourceTreeType
        {
            
            /// <summary> Frame information for this tree item. </summary>
            public Page.FrameType frame;
            /// <summary> Child frames. </summary>
            public object[] childFrames;
            /// <summary> Information about frame resources. </summary>
            public object[] resources;
        }
        public class FrameTreeType
        {
            
            /// <summary> Frame information for this tree item. </summary>
            public Page.FrameType frame;
            /// <summary> Child frames. </summary>
            public object[] childFrames;
        }
        public class NavigationEntryType
        {
            
            /// <summary> Unique id of the navigation history entry. </summary>
            public int id;
            /// <summary> URL of the navigation history entry. </summary>
            public string url;
            /// <summary> URL that the user typed in the url bar. </summary>
            public string userTypedURL;
            /// <summary> Title of the navigation history entry. </summary>
            public string title;
            /// <summary> Transition type. </summary>
            public string transitionType;
        }
        public class ScreencastFrameMetadataType
        {
            
            /// <summary> Top offset in DIP. </summary>
            public double offsetTop;
            /// <summary> Page scale factor. </summary>
            public double pageScaleFactor;
            /// <summary> Device screen width in DIP. </summary>
            public double deviceWidth;
            /// <summary> Device screen height in DIP. </summary>
            public double deviceHeight;
            /// <summary> Position of horizontal scroll in CSS pixels. </summary>
            public double scrollOffsetX;
            /// <summary> Position of vertical scroll in CSS pixels. </summary>
            public double scrollOffsetY;
            /// <summary> Frame swap timestamp. </summary>
            public double timestamp;
        }
        public class AppManifestErrorType
        {
            
            /// <summary> Error message. </summary>
            public string message;
            /// <summary> If criticial, this is a non-recoverable parse error. </summary>
            public int critical;
            /// <summary> Error line. </summary>
            public int line;
            /// <summary> Error column. </summary>
            public int column;
        }
        public class AppManifestParsedPropertiesType
        {
            
            /// <summary> Computed scope value </summary>
            public string scope;
        }
        public class LayoutViewportType
        {
            
            /// <summary> Horizontal offset relative to the document (CSS pixels). </summary>
            public int pageX;
            /// <summary> Vertical offset relative to the document (CSS pixels). </summary>
            public int pageY;
            /// <summary> Width (CSS pixels), excludes scrollbar if present. </summary>
            public int clientWidth;
            /// <summary> Height (CSS pixels), excludes scrollbar if present. </summary>
            public int clientHeight;
        }
        public class VisualViewportType
        {
            
            /// <summary> Horizontal offset relative to the layout viewport (CSS pixels). </summary>
            public double offsetX;
            /// <summary> Vertical offset relative to the layout viewport (CSS pixels). </summary>
            public double offsetY;
            /// <summary> Horizontal offset relative to the document (CSS pixels). </summary>
            public double pageX;
            /// <summary> Vertical offset relative to the document (CSS pixels). </summary>
            public double pageY;
            /// <summary> Width (CSS pixels), excludes scrollbar if present. </summary>
            public double clientWidth;
            /// <summary> Height (CSS pixels), excludes scrollbar if present. </summary>
            public double clientHeight;
            /// <summary> Scale relative to the ideal viewport (size at width=device-width). </summary>
            public double scale;
            /// <summary> Page zoom factor (CSS to device independent pixels ratio). </summary>
            public double zoom;
        }
        public class ViewportType
        {
            
            /// <summary> X offset in device independent pixels (dip). </summary>
            public double x;
            /// <summary> Y offset in device independent pixels (dip). </summary>
            public double y;
            /// <summary> Rectangle width in device independent pixels (dip). </summary>
            public double width;
            /// <summary> Rectangle height in device independent pixels (dip). </summary>
            public double height;
            /// <summary> Page scale factor. </summary>
            public double scale;
        }
        public class FontFamiliesType
        {
            
            /// <summary> The standard font-family. </summary>
            public string standard;
            /// <summary> The fixed font-family. </summary>
            public string @fixed;
            /// <summary> The serif font-family. </summary>
            public string serif;
            /// <summary> The sansSerif font-family. </summary>
            public string sansSerif;
            /// <summary> The cursive font-family. </summary>
            public string cursive;
            /// <summary> The fantasy font-family. </summary>
            public string fantasy;
            /// <summary> The pictograph font-family. </summary>
            public string pictograph;
        }
        public class ScriptFontFamiliesType
        {
            
            /// <summary> Name of the script which these font families are defined for. </summary>
            public string script;
            /// <summary> Generic font families collection for the script. </summary>
            public Page.FontFamiliesType fontFamilies;
        }
        public class FontSizesType
        {
            
            /// <summary> Default standard font size. </summary>
            public int standard;
            /// <summary> Default fixed font size. </summary>
            public int @fixed;
        }
        public class InstallabilityErrorArgumentType
        {
            
            /// <summary> Argument name (e.g. name:'minimum-icon-size-in-pixels'). </summary>
            public string name;
            /// <summary> Argument value (e.g. value:'64'). </summary>
            public string value;
        }
        public class InstallabilityErrorType
        {
            
            /// <summary> The error id (e.g. 'manifest-missing-suitable-icon'). </summary>
            public string errorId;
            /// <summary> The list of error arguments (e.g. {name:'minimum-icon-size-in-pixels', value:'64'}). </summary>
            public object[] errorArguments;
        }
        public class CompilationCacheParamsType
        {
            
            /// <summary> The URL of the script to produce a compilation cache entry for. </summary>
            public string url;
            /// <summary> A hint to the backend whether eager compilation is recommended.(the actual compilation mode used is upon backend discretion). </summary>
            public bool eager;
        }
        public class BackForwardCacheNotRestoredExplanationType
        {
            
            /// <summary> Type of the reason </summary>
            public string type;
            /// <summary> Not restored reason </summary>
            public string reason;
        }
        public class BackForwardCacheNotRestoredExplanationTreeType
        {
            
            /// <summary> URL of each frame </summary>
            public string url;
            /// <summary> Not restored reasons of each frame </summary>
            public object[] explanations;
            /// <summary> Array of children frame </summary>
            public object[] children;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnDomContentEventFiredParameters
        {
            
            /// <summary> [Require]  </summary>
            public double timestamp;
        }
        public class OnFileChooserOpenedParameters
        {
            
            /// <summary> [Require] Id of the frame containing input node. </summary>
            public string frameId;
            /// <summary> [Require] Input node id. </summary>
            public int backendNodeId;
            /// <summary> [Require] Input mode. </summary>
            public string mode;
        }
        public class OnFrameAttachedParameters
        {
            
            /// <summary> [Require] Id of the frame that has been attached. </summary>
            public string frameId;
            /// <summary> [Require] Parent frame identifier. </summary>
            public string parentFrameId;
            /// <summary> [Optional] JavaScript stack trace of when frame was attached, only set if frame initiated from script. </summary>
            public Runtime.StackTraceType stack;
        }
        public class OnFrameClearedScheduledNavigationParameters
        {
            
            /// <summary> [Require] Id of the frame that has cleared its scheduled navigation. </summary>
            public string frameId;
        }
        public class OnFrameDetachedParameters
        {
            
            /// <summary> [Require] Id of the frame that has been detached. </summary>
            public string frameId;
            /// <summary> [Require]  </summary>
            public string reason;
        }
        public class OnFrameNavigatedParameters
        {
            
            /// <summary> [Require] Frame object. </summary>
            public Page.FrameType frame;
            /// <summary> [Require]  </summary>
            public string type;
        }
        public class OnDocumentOpenedParameters
        {
            
            /// <summary> [Require] Frame object. </summary>
            public Page.FrameType frame;
        }
        public class OnFrameRequestedNavigationParameters
        {
            
            /// <summary> [Require] Id of the frame that is being navigated. </summary>
            public string frameId;
            /// <summary> [Require] The reason for the navigation. </summary>
            public string reason;
            /// <summary> [Require] The destination URL for the requested navigation. </summary>
            public string url;
            /// <summary> [Require] The disposition for the navigation. </summary>
            public string disposition;
        }
        public class OnFrameScheduledNavigationParameters
        {
            
            /// <summary> [Require] Id of the frame that has scheduled a navigation. </summary>
            public string frameId;
            /// <summary> [Require] Delay (in seconds) until the navigation is scheduled to begin. The navigation is notguaranteed to start. </summary>
            public double delay;
            /// <summary> [Require] The reason for the navigation. </summary>
            public string reason;
            /// <summary> [Require] The destination URL for the scheduled navigation. </summary>
            public string url;
        }
        public class OnFrameStartedLoadingParameters
        {
            
            /// <summary> [Require] Id of the frame that has started loading. </summary>
            public string frameId;
        }
        public class OnFrameStoppedLoadingParameters
        {
            
            /// <summary> [Require] Id of the frame that has stopped loading. </summary>
            public string frameId;
        }
        public class OnDownloadWillBeginParameters
        {
            
            /// <summary> [Require] Id of the frame that caused download to begin. </summary>
            public string frameId;
            /// <summary> [Require] Global unique identifier of the download. </summary>
            public string guid;
            /// <summary> [Require] URL of the resource being downloaded. </summary>
            public string url;
            /// <summary> [Require] Suggested file name of the resource (the actual name of the file saved on disk may differ). </summary>
            public string suggestedFilename;
        }
        public class OnDownloadProgressParameters
        {
            
            /// <summary> [Require] Global unique identifier of the download. </summary>
            public string guid;
            /// <summary> [Require] Total expected bytes to download. </summary>
            public double totalBytes;
            /// <summary> [Require] Total bytes received. </summary>
            public double receivedBytes;
            /// <summary> [Require] Download status. </summary>
            public string state;
        }
        public class OnJavascriptDialogClosedParameters
        {
            
            /// <summary> [Require] Whether dialog was confirmed. </summary>
            public bool result;
            /// <summary> [Require] User input in case of prompt. </summary>
            public string userInput;
        }
        public class OnJavascriptDialogOpeningParameters
        {
            
            /// <summary> [Require] Frame url. </summary>
            public string url;
            /// <summary> [Require] Message that will be displayed by the dialog. </summary>
            public string message;
            /// <summary> [Require] Dialog type. </summary>
            public string type;
            /// <summary> [Require] True iff browser is capable showing or acting on the given dialog. When browser has nodialog handler for given target, calling alert while Page domain is engaged will stallthe page execution. Execution can be resumed via calling Page.handleJavaScriptDialog. </summary>
            public bool hasBrowserHandler;
            /// <summary> [Optional] Default dialog prompt. </summary>
            public string defaultPrompt;
        }
        public class OnLifecycleEventParameters
        {
            
            /// <summary> [Require] Id of the frame. </summary>
            public string frameId;
            /// <summary> [Require] Loader identifier. Empty string if the request is fetched from worker. </summary>
            public string loaderId;
            /// <summary> [Require]  </summary>
            public string name;
            /// <summary> [Require]  </summary>
            public double timestamp;
        }
        public class OnBackForwardCacheNotUsedParameters
        {
            
            /// <summary> [Require] The loader id for the associated navgation. </summary>
            public string loaderId;
            /// <summary> [Require] The frame id of the associated frame. </summary>
            public string frameId;
            /// <summary> [Require] Array of reasons why the page could not be cached. This must not be empty. </summary>
            public object[] notRestoredExplanations;
            /// <summary> [Optional] Tree structure of reasons why the page could not be cached for each frame. </summary>
            public Page.BackForwardCacheNotRestoredExplanationTreeType notRestoredExplanationsTree;
        }
        public class OnLoadEventFiredParameters
        {
            
            /// <summary> [Require]  </summary>
            public double timestamp;
        }
        public class OnNavigatedWithinDocumentParameters
        {
            
            /// <summary> [Require] Id of the frame. </summary>
            public string frameId;
            /// <summary> [Require] Frame's new url. </summary>
            public string url;
        }
        public class OnScreencastFrameParameters
        {
            
            /// <summary> [Require] Base64-encoded compressed image. (Encoded as a base64 string when passed over JSON) </summary>
            public string data;
            /// <summary> [Require] Screencast frame metadata. </summary>
            public Page.ScreencastFrameMetadataType metadata;
            /// <summary> [Require] Frame number. </summary>
            public int sessionId;
        }
        public class OnScreencastVisibilityChangedParameters
        {
            
            /// <summary> [Require] True if the page is visible. </summary>
            public bool visible;
        }
        public class OnWindowOpenParameters
        {
            
            /// <summary> [Require] The URL for the new window. </summary>
            public string url;
            /// <summary> [Require] Window name. </summary>
            public string windowName;
            /// <summary> [Require] An array of enabled window features. </summary>
            public object[] windowFeatures;
            /// <summary> [Require] Whether or not it was triggered by user gesture. </summary>
            public bool userGesture;
        }
        public class OnCompilationCacheProducedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string url;
            /// <summary> [Require] Base64-encoded data (Encoded as a base64 string when passed over JSON) </summary>
            public string data;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class AddScriptToEvaluateOnLoadParameters
        {
            
            /// <summary> [Require]  </summary>
            public string scriptSource;
        }
        public class AddScriptToEvaluateOnNewDocumentParameters
        {
            
            /// <summary> [Require]  </summary>
            public string source;
            /// <summary> [Optional] If specified, creates an isolated world with the given name and evaluates given script in it.This world name will be used as the ExecutionContextDescription::name when the correspondingevent is emitted. </summary>
            public string worldName;
            /// <summary> [Optional] Specifies whether command line API should be available to the script, defaultsto false. </summary>
            public bool includeCommandLineAPI;
        }
        public class CaptureScreenshotParameters
        {
            
            /// <summary> [Optional] Image compression format (defaults to png). </summary>
            public string format;
            /// <summary> [Optional] Compression quality from range [0..100] (jpeg only). </summary>
            public int quality;
            /// <summary> [Optional] Capture the screenshot of a given region only. </summary>
            public Page.ViewportType clip;
            /// <summary> [Optional] Capture the screenshot from the surface, rather than the view. Defaults to true. </summary>
            public bool fromSurface;
            /// <summary> [Optional] Capture the screenshot beyond the viewport. Defaults to false. </summary>
            public bool captureBeyondViewport;
        }
        public class CaptureSnapshotParameters
        {
            
            /// <summary> [Optional] Format (defaults to mhtml). </summary>
            public string format;
        }
        public class CreateIsolatedWorldParameters
        {
            
            /// <summary> [Require] Id of the frame in which the isolated world should be created. </summary>
            public string frameId;
            /// <summary> [Optional] An optional name which is reported in the Execution Context. </summary>
            public string worldName;
            /// <summary> [Optional] Whether or not universal access should be granted to the isolated world. This is a powerfuloption, use with caution. </summary>
            public bool grantUniveralAccess;
        }
        public class DeleteCookieParameters
        {
            
            /// <summary> [Require] Name of the cookie to remove. </summary>
            public string cookieName;
            /// <summary> [Require] URL to match cooke domain and path. </summary>
            public string url;
        }
        public class GetResourceContentParameters
        {
            
            /// <summary> [Require] Frame id to get resource for. </summary>
            public string frameId;
            /// <summary> [Require] URL of the resource to get content for. </summary>
            public string url;
        }
        public class HandleJavaScriptDialogParameters
        {
            
            /// <summary> [Require] Whether to accept or dismiss the dialog. </summary>
            public bool accept;
            /// <summary> [Optional] The text to enter into the dialog prompt before accepting. Used only if this is a promptdialog. </summary>
            public string promptText;
        }
        public class NavigateParameters
        {
            
            /// <summary> [Require] URL to navigate the page to. </summary>
            public string url;
            /// <summary> [Optional] Referrer URL. </summary>
            public string referrer;
            /// <summary> [Optional] Intended transition type. </summary>
            public string transitionType;
            /// <summary> [Optional] Frame id to navigate, if not specified navigates the top frame. </summary>
            public string frameId;
            /// <summary> [Optional] Referrer-policy used for the navigation. </summary>
            public string referrerPolicy;
        }
        public class NavigateToHistoryEntryParameters
        {
            
            /// <summary> [Require] Unique id of the entry to navigate to. </summary>
            public int entryId;
        }
        public class PrintToPDFParameters
        {
            
            /// <summary> [Optional] Paper orientation. Defaults to false. </summary>
            public bool landscape;
            /// <summary> [Optional] Display header and footer. Defaults to false. </summary>
            public bool displayHeaderFooter;
            /// <summary> [Optional] Print background graphics. Defaults to false. </summary>
            public bool printBackground;
            /// <summary> [Optional] Scale of the webpage rendering. Defaults to 1. </summary>
            public double scale;
            /// <summary> [Optional] Paper width in inches. Defaults to 8.5 inches. </summary>
            public double paperWidth;
            /// <summary> [Optional] Paper height in inches. Defaults to 11 inches. </summary>
            public double paperHeight;
            /// <summary> [Optional] Top margin in inches. Defaults to 1cm (~0.4 inches). </summary>
            public double marginTop;
            /// <summary> [Optional] Bottom margin in inches. Defaults to 1cm (~0.4 inches). </summary>
            public double marginBottom;
            /// <summary> [Optional] Left margin in inches. Defaults to 1cm (~0.4 inches). </summary>
            public double marginLeft;
            /// <summary> [Optional] Right margin in inches. Defaults to 1cm (~0.4 inches). </summary>
            public double marginRight;
            /// <summary> [Optional] Paper ranges to print, e.g., '1-5, 8, 11-13'. Defaults to the empty string, which meansprint all pages. </summary>
            public string pageRanges;
            /// <summary> [Optional] Whether to silently ignore invalid but successfully parsed page ranges, such as '3-2'.Defaults to false. </summary>
            public bool ignoreInvalidPageRanges;
            /// <summary> [Optional] HTML template for the print header. Should be valid HTML markup with followingclasses used to inject printing values into them:- `date`: formatted print date- `title`: document title- `url`: document location- `pageNumber`: current page number- `totalPages`: total pages in the documentFor example, `<span class=title></span>` would generate span containing the title. </summary>
            public string headerTemplate;
            /// <summary> [Optional] HTML template for the print footer. Should use the same format as the `headerTemplate`. </summary>
            public string footerTemplate;
            /// <summary> [Optional] Whether or not to prefer page size as defined by css. Defaults to false,in which case the content will be scaled to fit the paper size. </summary>
            public bool preferCSSPageSize;
            /// <summary> [Optional] return as stream </summary>
            public string transferMode;
        }
        public class ReloadParameters
        {
            
            /// <summary> [Optional] If true, browser cache is ignored (as if the user pressed Shift+refresh). </summary>
            public bool ignoreCache;
            /// <summary> [Optional] If set, the script will be injected into all frames of the inspected page after reload.Argument will be ignored if reloading dataURL origin. </summary>
            public string scriptToEvaluateOnLoad;
        }
        public class RemoveScriptToEvaluateOnLoadParameters
        {
            
            /// <summary> [Require]  </summary>
            public string identifier;
        }
        public class RemoveScriptToEvaluateOnNewDocumentParameters
        {
            
            /// <summary> [Require]  </summary>
            public string identifier;
        }
        public class ScreencastFrameAckParameters
        {
            
            /// <summary> [Require] Frame number. </summary>
            public int sessionId;
        }
        public class SearchInResourceParameters
        {
            
            /// <summary> [Require] Frame id for resource to search in. </summary>
            public string frameId;
            /// <summary> [Require] URL of the resource to search in. </summary>
            public string url;
            /// <summary> [Require] String to search for. </summary>
            public string query;
            /// <summary> [Optional] If true, search is case sensitive. </summary>
            public bool caseSensitive;
            /// <summary> [Optional] If true, treats string parameter as regex. </summary>
            public bool isRegex;
        }
        public class SetAdBlockingEnabledParameters
        {
            
            /// <summary> [Require] Whether to block ads. </summary>
            public bool enabled;
        }
        public class SetBypassCSPParameters
        {
            
            /// <summary> [Require] Whether to bypass page CSP. </summary>
            public bool enabled;
        }
        public class GetPermissionsPolicyStateParameters
        {
            
            /// <summary> [Require]  </summary>
            public string frameId;
        }
        public class GetOriginTrialsParameters
        {
            
            /// <summary> [Require]  </summary>
            public string frameId;
        }
        public class SetDeviceMetricsOverrideParameters
        {
            
            /// <summary> [Require] Overriding width value in pixels (minimum 0, maximum 10000000). 0 disables the override. </summary>
            public int width;
            /// <summary> [Require] Overriding height value in pixels (minimum 0, maximum 10000000). 0 disables the override. </summary>
            public int height;
            /// <summary> [Require] Overriding device scale factor value. 0 disables the override. </summary>
            public double deviceScaleFactor;
            /// <summary> [Require] Whether to emulate mobile device. This includes viewport meta tag, overlay scrollbars, textautosizing and more. </summary>
            public bool mobile;
            /// <summary> [Optional] Scale to apply to resulting view image. </summary>
            public double scale;
            /// <summary> [Optional] Overriding screen width value in pixels (minimum 0, maximum 10000000). </summary>
            public int screenWidth;
            /// <summary> [Optional] Overriding screen height value in pixels (minimum 0, maximum 10000000). </summary>
            public int screenHeight;
            /// <summary> [Optional] Overriding view X position on screen in pixels (minimum 0, maximum 10000000). </summary>
            public int positionX;
            /// <summary> [Optional] Overriding view Y position on screen in pixels (minimum 0, maximum 10000000). </summary>
            public int positionY;
            /// <summary> [Optional] Do not set visible view size, rely upon explicit setVisibleSize call. </summary>
            public bool dontSetVisibleSize;
            /// <summary> [Optional] Screen orientation override. </summary>
            public Emulation.ScreenOrientationType screenOrientation;
            /// <summary> [Optional] The viewport dimensions and scale. If not set, the override is cleared. </summary>
            public Page.ViewportType viewport;
        }
        public class SetDeviceOrientationOverrideParameters
        {
            
            /// <summary> [Require] Mock alpha </summary>
            public double alpha;
            /// <summary> [Require] Mock beta </summary>
            public double beta;
            /// <summary> [Require] Mock gamma </summary>
            public double gamma;
        }
        public class SetFontFamiliesParameters
        {
            
            /// <summary> [Require] Specifies font families to set. If a font family is not specified, it won't be changed. </summary>
            public Page.FontFamiliesType fontFamilies;
            /// <summary> [Optional] Specifies font families to set for individual scripts. </summary>
            public object[] forScripts;
        }
        public class SetFontSizesParameters
        {
            
            /// <summary> [Require] Specifies font sizes to set. If a font size is not specified, it won't be changed. </summary>
            public Page.FontSizesType fontSizes;
        }
        public class SetDocumentContentParameters
        {
            
            /// <summary> [Require] Frame id to set HTML for. </summary>
            public string frameId;
            /// <summary> [Require] HTML content to set. </summary>
            public string html;
        }
        public class SetDownloadBehaviorParameters
        {
            
            /// <summary> [Require] Whether to allow all or deny all download requests, or use default Chrome behavior ifavailable (otherwise deny). </summary>
            public string behavior;
            /// <summary> [Optional] The default path to save downloaded files to. This is required if behavior is set to 'allow' </summary>
            public string downloadPath;
        }
        public class SetGeolocationOverrideParameters
        {
            
            /// <summary> [Optional] Mock latitude </summary>
            public double latitude;
            /// <summary> [Optional] Mock longitude </summary>
            public double longitude;
            /// <summary> [Optional] Mock accuracy </summary>
            public double accuracy;
        }
        public class SetLifecycleEventsEnabledParameters
        {
            
            /// <summary> [Require] If true, starts emitting lifecycle events. </summary>
            public bool enabled;
        }
        public class SetTouchEmulationEnabledParameters
        {
            
            /// <summary> [Require] Whether the touch event emulation should be enabled. </summary>
            public bool enabled;
            /// <summary> [Optional] Touch/gesture events configuration. Default: current platform. </summary>
            public string configuration;
        }
        public class StartScreencastParameters
        {
            
            /// <summary> [Optional] Image compression format. </summary>
            public string format;
            /// <summary> [Optional] Compression quality from range [0..100]. </summary>
            public int quality;
            /// <summary> [Optional] Maximum screenshot width. </summary>
            public int maxWidth;
            /// <summary> [Optional] Maximum screenshot height. </summary>
            public int maxHeight;
            /// <summary> [Optional] Send every n-th frame. </summary>
            public int everyNthFrame;
        }
        public class SetWebLifecycleStateParameters
        {
            
            /// <summary> [Require] Target lifecycle state </summary>
            public string state;
        }
        public class ProduceCompilationCacheParameters
        {
            
            /// <summary> [Require]  </summary>
            public object[] scripts;
        }
        public class AddCompilationCacheParameters
        {
            
            /// <summary> [Require]  </summary>
            public string url;
            /// <summary> [Require] Base64-encoded data (Encoded as a base64 string when passed over JSON) </summary>
            public string data;
        }
        public class SetSPCTransactionModeParameters
        {
            
            /// <summary> [Require]  </summary>
            public string mode;
        }
        public class GenerateTestReportParameters
        {
            
            /// <summary> [Require] Message to be displayed in the report. </summary>
            public string message;
            /// <summary> [Optional] Specifies the endpoint group to deliver the report to. </summary>
            public string group;
        }
        public class SetInterceptFileChooserDialogParameters
        {
            
            /// <summary> [Require]  </summary>
            public bool enabled;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class AddScriptToEvaluateOnLoadReturn
        {
            
            /// <summary> Identifier of the added script. </summary>
            public string identifier;
        }
        public class AddScriptToEvaluateOnNewDocumentReturn
        {
            
            /// <summary> Identifier of the added script. </summary>
            public string identifier;
        }
        public class CaptureScreenshotReturn
        {
            
            /// <summary> Base64-encoded image data. (Encoded as a base64 string when passed over JSON) </summary>
            public string data;
        }
        public class CaptureSnapshotReturn
        {
            
            /// <summary> Serialized page data. </summary>
            public string data;
        }
        public class CreateIsolatedWorldReturn
        {
            
            /// <summary> Execution context of the isolated world. </summary>
            public int executionContextId;
        }
        public class GetAppManifestReturn
        {
            
            /// <summary> Manifest location. </summary>
            public string url;
            /// <summary>  </summary>
            public object[] errors;
            /// <summary> Manifest content. </summary>
            public string data;
            /// <summary> Parsed manifest properties </summary>
            public Page.AppManifestParsedPropertiesType parsed;
        }
        public class GetInstallabilityErrorsReturn
        {
            
            /// <summary>  </summary>
            public object[] installabilityErrors;
        }
        public class GetManifestIconsReturn
        {
            
            /// <summary>  </summary>
            public string primaryIcon;
        }
        public class GetAppIdReturn
        {
            
            /// <summary> App id, either from manifest's id attribute or computed from start_url </summary>
            public string appId;
            /// <summary> Recommendation for manifest's id attribute to match current id computed from start_url </summary>
            public string recommendedId;
        }
        public class GetCookiesReturn
        {
            
            /// <summary> Array of cookie objects. </summary>
            public object[] cookies;
        }
        public class GetFrameTreeReturn
        {
            
            /// <summary> Present frame tree structure. </summary>
            public Page.FrameTreeType frameTree;
        }
        public class GetLayoutMetricsReturn
        {
            
            /// <summary> Deprecated metrics relating to the layout viewport. Can be in DP or in CSS pixels depending on the `enable-use-zoom-for-dsf` flag. Use `cssLayoutViewport` instead. </summary>
            public Page.LayoutViewportType layoutViewport;
            /// <summary> Deprecated metrics relating to the visual viewport. Can be in DP or in CSS pixels depending on the `enable-use-zoom-for-dsf` flag. Use `cssVisualViewport` instead. </summary>
            public Page.VisualViewportType visualViewport;
            /// <summary> Deprecated size of scrollable area. Can be in DP or in CSS pixels depending on the `enable-use-zoom-for-dsf` flag. Use `cssContentSize` instead. </summary>
            public DOM.RectType contentSize;
            /// <summary> Metrics relating to the layout viewport in CSS pixels. </summary>
            public Page.LayoutViewportType cssLayoutViewport;
            /// <summary> Metrics relating to the visual viewport in CSS pixels. </summary>
            public Page.VisualViewportType cssVisualViewport;
            /// <summary> Size of scrollable area in CSS pixels. </summary>
            public DOM.RectType cssContentSize;
        }
        public class GetNavigationHistoryReturn
        {
            
            /// <summary> Index of the current navigation history entry. </summary>
            public int currentIndex;
            /// <summary> Array of navigation history entries. </summary>
            public object[] entries;
        }
        public class GetResourceContentReturn
        {
            
            /// <summary> Resource content. </summary>
            public string content;
            /// <summary> True, if content was served as base64. </summary>
            public bool base64Encoded;
        }
        public class GetResourceTreeReturn
        {
            
            /// <summary> Present frame / resource tree structure. </summary>
            public Page.FrameResourceTreeType frameTree;
        }
        public class NavigateReturn
        {
            
            /// <summary> Frame id that has navigated (or failed to navigate) </summary>
            public string frameId;
            /// <summary> Loader identifier. </summary>
            public string loaderId;
            /// <summary> User friendly error message, present if and only if navigation has failed. </summary>
            public string errorText;
        }
        public class PrintToPDFReturn
        {
            
            /// <summary> Base64-encoded pdf data. Empty if |returnAsStream| is specified. (Encoded as a base64 string when passed over JSON) </summary>
            public string data;
            /// <summary> A handle of the stream that holds resulting PDF data. </summary>
            public string stream;
        }
        public class SearchInResourceReturn
        {
            
            /// <summary> List of search matches. </summary>
            public object[] result;
        }
        public class GetPermissionsPolicyStateReturn
        {
            
            /// <summary>  </summary>
            public object[] states;
        }
        public class GetOriginTrialsReturn
        {
            
            /// <summary>  </summary>
            public object[] originTrials;
        }
    }
    
    public class Performance : DomainBase
    {
        public Performance(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Current values of the metrics. </summary>
        /// <returns> remove handler </returns>
        public Action OnMetrics(Action<OnMetricsParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnMetricsParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Performance.metrics" : $"Performance.metrics.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Disable collecting and reporting metrics. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Performance.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enable collecting and reporting metrics. 
        /// </summary>
        public async Task Enable(EnableParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Performance.enable", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets time domain to use for collecting and reporting duration metrics.Note that this must be called before enabling metrics collection. Callingthis method while metrics collection is enabled returns an error. 
        /// </summary>
        public async Task SetTimeDomain(SetTimeDomainParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Performance.setTimeDomain", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Retrieve current values of run-time metrics. 
        /// </summary>
        public async Task<GetMetricsReturn> GetMetrics(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Performance.getMetrics", null, sessionId);
            return Convert<GetMetricsReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class MetricType
        {
            
            /// <summary> Metric name. </summary>
            public string name;
            /// <summary> Metric value. </summary>
            public double value;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnMetricsParameters
        {
            
            /// <summary> [Require] Current values of the metrics. </summary>
            public object[] metrics;
            /// <summary> [Require] Timestamp title. </summary>
            public string title;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class EnableParameters
        {
            
            /// <summary> [Optional] Time domain to use for collecting and reporting duration metrics. </summary>
            public string timeDomain;
        }
        public class SetTimeDomainParameters
        {
            
            /// <summary> [Require] Time domain </summary>
            public string timeDomain;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetMetricsReturn
        {
            
            /// <summary> Current values for run-time metrics. </summary>
            public object[] metrics;
        }
    }
    
    public class PerformanceTimeline : DomainBase
    {
        public PerformanceTimeline(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Sent when a performance timeline event is added. See reportPerformanceTimeline method. </summary>
        /// <returns> remove handler </returns>
        public Action OnTimelineEventAdded(Action<OnTimelineEventAddedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnTimelineEventAddedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "PerformanceTimeline.timelineEventAdded" : $"PerformanceTimeline.timelineEventAdded.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Previously buffered events would be reported before method returns.See also: timelineEventAdded 
        /// </summary>
        public async Task Enable(EnableParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("PerformanceTimeline.enable", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class LargestContentfulPaintType
        {
            
            /// <summary>  </summary>
            public double renderTime;
            /// <summary>  </summary>
            public double loadTime;
            /// <summary> The number of pixels being painted. </summary>
            public double size;
            /// <summary> The id attribute of the element, if available. </summary>
            public string elementId;
            /// <summary> The URL of the image (may be trimmed). </summary>
            public string url;
            /// <summary>  </summary>
            public int nodeId;
        }
        public class LayoutShiftAttributionType
        {
            
            /// <summary>  </summary>
            public DOM.RectType previousRect;
            /// <summary>  </summary>
            public DOM.RectType currentRect;
            /// <summary>  </summary>
            public int nodeId;
        }
        public class LayoutShiftType
        {
            
            /// <summary> Score increment produced by this event. </summary>
            public double value;
            /// <summary>  </summary>
            public bool hadRecentInput;
            /// <summary>  </summary>
            public double lastInputTime;
            /// <summary>  </summary>
            public object[] sources;
        }
        public class TimelineEventType
        {
            
            /// <summary> Identifies the frame that this event is related to. Empty for non-frame targets. </summary>
            public string frameId;
            /// <summary> The event type, as specified in https://w3c.github.io/performance-timeline/#dom-performanceentry-entrytypeThis determines which of the optional "details" fiedls is present. </summary>
            public string type;
            /// <summary> Name may be empty depending on the type. </summary>
            public string name;
            /// <summary> Time in seconds since Epoch, monotonically increasing within document lifetime. </summary>
            public double time;
            /// <summary> Event duration, if applicable. </summary>
            public double duration;
            /// <summary>  </summary>
            public PerformanceTimeline.LargestContentfulPaintType lcpDetails;
            /// <summary>  </summary>
            public PerformanceTimeline.LayoutShiftType layoutShiftDetails;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnTimelineEventAddedParameters
        {
            
            /// <summary> [Require]  </summary>
            public PerformanceTimeline.TimelineEventType @event;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class EnableParameters
        {
            
            /// <summary> [Require] The types of event to report, as specified inhttps://w3c.github.io/performance-timeline/#dom-performanceentry-entrytypeThe specified filter overrides any previous filters, passing emptyfilter disables recording.Note that not all types exposed to the web platform are currently supported. </summary>
            public object[] eventTypes;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class Security : DomainBase
    {
        public Security(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> There is a certificate error. If overriding certificate errors is enabled, then it should behandled with the `handleCertificateError` command. Note: this event does not fire if thecertificate error has been allowed internally. Only one client per target should overridecertificate errors at the same time. </summary>
        /// <returns> remove handler </returns>
        public Action OnCertificateError(Action<OnCertificateErrorParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnCertificateErrorParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Security.certificateError" : $"Security.certificateError.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> The security state of the page changed. </summary>
        /// <returns> remove handler </returns>
        public Action OnVisibleSecurityStateChanged(Action<OnVisibleSecurityStateChangedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnVisibleSecurityStateChangedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Security.visibleSecurityStateChanged" : $"Security.visibleSecurityStateChanged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> The security state of the page changed. No longer being sent. </summary>
        /// <returns> remove handler </returns>
        public Action OnSecurityStateChanged(Action<OnSecurityStateChangedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnSecurityStateChangedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Security.securityStateChanged" : $"Security.securityStateChanged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Disables tracking security state changes. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Security.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables tracking security state changes. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Security.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enable/disable whether all certificate errors should be ignored. 
        /// </summary>
        public async Task SetIgnoreCertificateErrors(SetIgnoreCertificateErrorsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Security.setIgnoreCertificateErrors", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Handles a certificate error that fired a certificateError event. 
        /// </summary>
        public async Task HandleCertificateError(HandleCertificateErrorParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Security.handleCertificateError", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Enable/disable overriding certificate errors. If enabled, all certificate error events need tobe handled by the DevTools client and should be answered with `handleCertificateError` commands. 
        /// </summary>
        public async Task SetOverrideCertificateErrors(SetOverrideCertificateErrorsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Security.setOverrideCertificateErrors", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class CertificateSecurityStateType
        {
            
            /// <summary> Protocol name (e.g. "TLS 1.2" or "QUIC"). </summary>
            public string protocol;
            /// <summary> Key Exchange used by the connection, or the empty string if not applicable. </summary>
            public string keyExchange;
            /// <summary> (EC)DH group used by the connection, if applicable. </summary>
            public string keyExchangeGroup;
            /// <summary> Cipher name. </summary>
            public string cipher;
            /// <summary> TLS MAC. Note that AEAD ciphers do not have separate MACs. </summary>
            public string mac;
            /// <summary> Page certificate. </summary>
            public object[] certificate;
            /// <summary> Certificate subject name. </summary>
            public string subjectName;
            /// <summary> Name of the issuing CA. </summary>
            public string issuer;
            /// <summary> Certificate valid from date. </summary>
            public double validFrom;
            /// <summary> Certificate valid to (expiration) date </summary>
            public double validTo;
            /// <summary> The highest priority network error code, if the certificate has an error. </summary>
            public string certificateNetworkError;
            /// <summary> True if the certificate uses a weak signature aglorithm. </summary>
            public bool certificateHasWeakSignature;
            /// <summary> True if the certificate has a SHA1 signature in the chain. </summary>
            public bool certificateHasSha1Signature;
            /// <summary> True if modern SSL </summary>
            public bool modernSSL;
            /// <summary> True if the connection is using an obsolete SSL protocol. </summary>
            public bool obsoleteSslProtocol;
            /// <summary> True if the connection is using an obsolete SSL key exchange. </summary>
            public bool obsoleteSslKeyExchange;
            /// <summary> True if the connection is using an obsolete SSL cipher. </summary>
            public bool obsoleteSslCipher;
            /// <summary> True if the connection is using an obsolete SSL signature. </summary>
            public bool obsoleteSslSignature;
        }
        public class SafetyTipInfoType
        {
            
            /// <summary> Describes whether the page triggers any safety tips or reputation warnings. Default is unknown. </summary>
            public string safetyTipStatus;
            /// <summary> The URL the safety tip suggested ("Did you mean?"). Only filled in for lookalike matches. </summary>
            public string safeUrl;
        }
        public class VisibleSecurityStateType
        {
            
            /// <summary> The security level of the page. </summary>
            public string securityState;
            /// <summary> Security state details about the page certificate. </summary>
            public Security.CertificateSecurityStateType certificateSecurityState;
            /// <summary> The type of Safety Tip triggered on the page. Note that this field will be set even if the Safety Tip UI was not actually shown. </summary>
            public Security.SafetyTipInfoType safetyTipInfo;
            /// <summary> Array of security state issues ids. </summary>
            public object[] securityStateIssueIds;
        }
        public class SecurityStateExplanationType
        {
            
            /// <summary> Security state representing the severity of the factor being explained. </summary>
            public string securityState;
            /// <summary> Title describing the type of factor. </summary>
            public string title;
            /// <summary> Short phrase describing the type of factor. </summary>
            public string summary;
            /// <summary> Full text explanation of the factor. </summary>
            public string description;
            /// <summary> The type of mixed content described by the explanation. </summary>
            public string mixedContentType;
            /// <summary> Page certificate. </summary>
            public object[] certificate;
            /// <summary> Recommendations to fix any issues. </summary>
            public object[] recommendations;
        }
        public class InsecureContentStatusType
        {
            
            /// <summary> Always false. </summary>
            public bool ranMixedContent;
            /// <summary> Always false. </summary>
            public bool displayedMixedContent;
            /// <summary> Always false. </summary>
            public bool containedMixedForm;
            /// <summary> Always false. </summary>
            public bool ranContentWithCertErrors;
            /// <summary> Always false. </summary>
            public bool displayedContentWithCertErrors;
            /// <summary> Always set to unknown. </summary>
            public string ranInsecureContentStyle;
            /// <summary> Always set to unknown. </summary>
            public string displayedInsecureContentStyle;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnCertificateErrorParameters
        {
            
            /// <summary> [Require] The ID of the event. </summary>
            public int eventId;
            /// <summary> [Require] The type of the error. </summary>
            public string errorType;
            /// <summary> [Require] The url that was requested. </summary>
            public string requestURL;
        }
        public class OnVisibleSecurityStateChangedParameters
        {
            
            /// <summary> [Require] Security state information about the page. </summary>
            public Security.VisibleSecurityStateType visibleSecurityState;
        }
        public class OnSecurityStateChangedParameters
        {
            
            /// <summary> [Require] Security state. </summary>
            public string securityState;
            /// <summary> [Require] True if the page was loaded over cryptographic transport such as HTTPS. </summary>
            public bool schemeIsCryptographic;
            /// <summary> [Require] Previously a list of explanations for the security state. Now alwaysempty. </summary>
            public object[] explanations;
            /// <summary> [Require] Information about insecure content on the page. </summary>
            public Security.InsecureContentStatusType insecureContentStatus;
            /// <summary> [Optional] Overrides user-visible description of the state. Always omitted. </summary>
            public string summary;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class SetIgnoreCertificateErrorsParameters
        {
            
            /// <summary> [Require] If true, all certificate errors will be ignored. </summary>
            public bool ignore;
        }
        public class HandleCertificateErrorParameters
        {
            
            /// <summary> [Require] The ID of the event. </summary>
            public int eventId;
            /// <summary> [Require] The action to take on the certificate error. </summary>
            public string action;
        }
        public class SetOverrideCertificateErrorsParameters
        {
            
            /// <summary> [Require] If true, certificate errors will be overridden. </summary>
            public bool @override;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class ServiceWorker : DomainBase
    {
        public ServiceWorker(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnWorkerErrorReported(Action<OnWorkerErrorReportedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWorkerErrorReportedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "ServiceWorker.workerErrorReported" : $"ServiceWorker.workerErrorReported.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnWorkerRegistrationUpdated(Action<OnWorkerRegistrationUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWorkerRegistrationUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "ServiceWorker.workerRegistrationUpdated" : $"ServiceWorker.workerRegistrationUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnWorkerVersionUpdated(Action<OnWorkerVersionUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnWorkerVersionUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "ServiceWorker.workerVersionUpdated" : $"ServiceWorker.workerVersionUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        ///  
        /// </summary>
        public async Task DeliverPushMessage(DeliverPushMessageParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.deliverPushMessage", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task DispatchSyncEvent(DispatchSyncEventParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.dispatchSyncEvent", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task DispatchPeriodicSyncEvent(DispatchPeriodicSyncEventParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.dispatchPeriodicSyncEvent", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task InspectWorker(InspectWorkerParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.inspectWorker", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetForceUpdateOnPageLoad(SetForceUpdateOnPageLoadParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.setForceUpdateOnPageLoad", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SkipWaiting(SkipWaitingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.skipWaiting", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task StartWorker(StartWorkerParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.startWorker", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task StopAllWorkers(string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.stopAllWorkers", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task StopWorker(StopWorkerParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.stopWorker", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task Unregister(UnregisterParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.unregister", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task UpdateRegistration(UpdateRegistrationParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("ServiceWorker.updateRegistration", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class ServiceWorkerRegistrationType
        {
            
            /// <summary>  </summary>
            public string registrationId;
            /// <summary>  </summary>
            public string scopeURL;
            /// <summary>  </summary>
            public bool isDeleted;
        }
        public class ServiceWorkerVersionType
        {
            
            /// <summary>  </summary>
            public string versionId;
            /// <summary>  </summary>
            public string registrationId;
            /// <summary>  </summary>
            public string scriptURL;
            /// <summary>  </summary>
            public string runningStatus;
            /// <summary>  </summary>
            public string status;
            /// <summary> The Last-Modified header value of the main script. </summary>
            public double scriptLastModified;
            /// <summary> The time at which the response headers of the main script were received from the server.For cached script it is the last time the cache entry was validated. </summary>
            public double scriptResponseTime;
            /// <summary>  </summary>
            public object[] controlledClients;
            /// <summary>  </summary>
            public string targetId;
        }
        public class ServiceWorkerErrorMessageType
        {
            
            /// <summary>  </summary>
            public string errorMessage;
            /// <summary>  </summary>
            public string registrationId;
            /// <summary>  </summary>
            public string versionId;
            /// <summary>  </summary>
            public string sourceURL;
            /// <summary>  </summary>
            public int lineNumber;
            /// <summary>  </summary>
            public int columnNumber;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnWorkerErrorReportedParameters
        {
            
            /// <summary> [Require]  </summary>
            public ServiceWorker.ServiceWorkerErrorMessageType errorMessage;
        }
        public class OnWorkerRegistrationUpdatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public object[] registrations;
        }
        public class OnWorkerVersionUpdatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public object[] versions;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class DeliverPushMessageParameters
        {
            
            /// <summary> [Require]  </summary>
            public string origin;
            /// <summary> [Require]  </summary>
            public string registrationId;
            /// <summary> [Require]  </summary>
            public string data;
        }
        public class DispatchSyncEventParameters
        {
            
            /// <summary> [Require]  </summary>
            public string origin;
            /// <summary> [Require]  </summary>
            public string registrationId;
            /// <summary> [Require]  </summary>
            public string tag;
            /// <summary> [Require]  </summary>
            public bool lastChance;
        }
        public class DispatchPeriodicSyncEventParameters
        {
            
            /// <summary> [Require]  </summary>
            public string origin;
            /// <summary> [Require]  </summary>
            public string registrationId;
            /// <summary> [Require]  </summary>
            public string tag;
        }
        public class InspectWorkerParameters
        {
            
            /// <summary> [Require]  </summary>
            public string versionId;
        }
        public class SetForceUpdateOnPageLoadParameters
        {
            
            /// <summary> [Require]  </summary>
            public bool forceUpdateOnPageLoad;
        }
        public class SkipWaitingParameters
        {
            
            /// <summary> [Require]  </summary>
            public string scopeURL;
        }
        public class StartWorkerParameters
        {
            
            /// <summary> [Require]  </summary>
            public string scopeURL;
        }
        public class StopWorkerParameters
        {
            
            /// <summary> [Require]  </summary>
            public string versionId;
        }
        public class UnregisterParameters
        {
            
            /// <summary> [Require]  </summary>
            public string scopeURL;
        }
        public class UpdateRegistrationParameters
        {
            
            /// <summary> [Require]  </summary>
            public string scopeURL;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class Storage : DomainBase
    {
        public Storage(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> A cache's contents have been modified. </summary>
        /// <returns> remove handler </returns>
        public Action OnCacheStorageContentUpdated(Action<OnCacheStorageContentUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnCacheStorageContentUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Storage.cacheStorageContentUpdated" : $"Storage.cacheStorageContentUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> A cache has been added/deleted. </summary>
        /// <returns> remove handler </returns>
        public Action OnCacheStorageListUpdated(Action<OnCacheStorageListUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnCacheStorageListUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Storage.cacheStorageListUpdated" : $"Storage.cacheStorageListUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> The origin's IndexedDB object store has been modified. </summary>
        /// <returns> remove handler </returns>
        public Action OnIndexedDBContentUpdated(Action<OnIndexedDBContentUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnIndexedDBContentUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Storage.indexedDBContentUpdated" : $"Storage.indexedDBContentUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> The origin's IndexedDB database list has been modified. </summary>
        /// <returns> remove handler </returns>
        public Action OnIndexedDBListUpdated(Action<OnIndexedDBListUpdatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnIndexedDBListUpdatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Storage.indexedDBListUpdated" : $"Storage.indexedDBListUpdated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> One of the interest groups was accessed by the associated page. </summary>
        /// <returns> remove handler </returns>
        public Action OnInterestGroupAccessed(Action<OnInterestGroupAccessedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnInterestGroupAccessedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Storage.interestGroupAccessed" : $"Storage.interestGroupAccessed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Clears storage for origin. 
        /// </summary>
        public async Task ClearDataForOrigin(ClearDataForOriginParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.clearDataForOrigin", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns all browser cookies. 
        /// </summary>
        public async Task<GetCookiesReturn> GetCookies(GetCookiesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.getCookies", parameters, sessionId);
            return Convert<GetCookiesReturn>(___r);
        }
        /// <summary> 
        /// Sets given cookies. 
        /// </summary>
        public async Task SetCookies(SetCookiesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.setCookies", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears cookies. 
        /// </summary>
        public async Task ClearCookies(ClearCookiesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.clearCookies", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns usage and quota in bytes. 
        /// </summary>
        public async Task<GetUsageAndQuotaReturn> GetUsageAndQuota(GetUsageAndQuotaParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.getUsageAndQuota", parameters, sessionId);
            return Convert<GetUsageAndQuotaReturn>(___r);
        }
        /// <summary> 
        /// Override quota for the specified origin 
        /// </summary>
        public async Task OverrideQuotaForOrigin(OverrideQuotaForOriginParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.overrideQuotaForOrigin", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Registers origin to be notified when an update occurs to its cache storage list. 
        /// </summary>
        public async Task TrackCacheStorageForOrigin(TrackCacheStorageForOriginParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.trackCacheStorageForOrigin", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Registers origin to be notified when an update occurs to its IndexedDB. 
        /// </summary>
        public async Task TrackIndexedDBForOrigin(TrackIndexedDBForOriginParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.trackIndexedDBForOrigin", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Unregisters origin from receiving notifications for cache storage. 
        /// </summary>
        public async Task UntrackCacheStorageForOrigin(UntrackCacheStorageForOriginParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.untrackCacheStorageForOrigin", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Unregisters origin from receiving notifications for IndexedDB. 
        /// </summary>
        public async Task UntrackIndexedDBForOrigin(UntrackIndexedDBForOriginParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.untrackIndexedDBForOrigin", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns the number of stored Trust Tokens per issuer for thecurrent browsing context. 
        /// </summary>
        public async Task<GetTrustTokensReturn> GetTrustTokens(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.getTrustTokens", null, sessionId);
            return Convert<GetTrustTokensReturn>(___r);
        }
        /// <summary> 
        /// Removes all Trust Tokens issued by the provided issuerOrigin.Leaves other stored data, including the issuer's Redemption Records, intact. 
        /// </summary>
        public async Task<ClearTrustTokensReturn> ClearTrustTokens(ClearTrustTokensParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.clearTrustTokens", parameters, sessionId);
            return Convert<ClearTrustTokensReturn>(___r);
        }
        /// <summary> 
        /// Gets details for a named interest group. 
        /// </summary>
        public async Task<GetInterestGroupDetailsReturn> GetInterestGroupDetails(GetInterestGroupDetailsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.getInterestGroupDetails", parameters, sessionId);
            return Convert<GetInterestGroupDetailsReturn>(___r);
        }
        /// <summary> 
        /// Enables/Disables issuing of interestGroupAccessed events. 
        /// </summary>
        public async Task SetInterestGroupTracking(SetInterestGroupTrackingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Storage.setInterestGroupTracking", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class UsageForTypeType
        {
            
            /// <summary> Name of storage type. </summary>
            public string storageType;
            /// <summary> Storage usage (bytes). </summary>
            public double usage;
        }
        public class TrustTokensType
        {
            
            /// <summary>  </summary>
            public string issuerOrigin;
            /// <summary>  </summary>
            public double count;
        }
        public class InterestGroupAdType
        {
            
            /// <summary>  </summary>
            public string renderUrl;
            /// <summary>  </summary>
            public string metadata;
        }
        public class InterestGroupDetailsType
        {
            
            /// <summary>  </summary>
            public string ownerOrigin;
            /// <summary>  </summary>
            public string name;
            /// <summary>  </summary>
            public double expirationTime;
            /// <summary>  </summary>
            public string joiningOrigin;
            /// <summary>  </summary>
            public string biddingUrl;
            /// <summary>  </summary>
            public string biddingWasmHelperUrl;
            /// <summary>  </summary>
            public string updateUrl;
            /// <summary>  </summary>
            public string trustedBiddingSignalsUrl;
            /// <summary>  </summary>
            public object[] trustedBiddingSignalsKeys;
            /// <summary>  </summary>
            public string userBiddingSignals;
            /// <summary>  </summary>
            public object[] ads;
            /// <summary>  </summary>
            public object[] adComponents;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnCacheStorageContentUpdatedParameters
        {
            
            /// <summary> [Require] Origin to update. </summary>
            public string origin;
            /// <summary> [Require] Name of cache in origin. </summary>
            public string cacheName;
        }
        public class OnCacheStorageListUpdatedParameters
        {
            
            /// <summary> [Require] Origin to update. </summary>
            public string origin;
        }
        public class OnIndexedDBContentUpdatedParameters
        {
            
            /// <summary> [Require] Origin to update. </summary>
            public string origin;
            /// <summary> [Require] Database to update. </summary>
            public string databaseName;
            /// <summary> [Require] ObjectStore to update. </summary>
            public string objectStoreName;
        }
        public class OnIndexedDBListUpdatedParameters
        {
            
            /// <summary> [Require] Origin to update. </summary>
            public string origin;
        }
        public class OnInterestGroupAccessedParameters
        {
            
            /// <summary> [Require]  </summary>
            public double accessTime;
            /// <summary> [Require]  </summary>
            public string type;
            /// <summary> [Require]  </summary>
            public string ownerOrigin;
            /// <summary> [Require]  </summary>
            public string name;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class ClearDataForOriginParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string origin;
            /// <summary> [Require] Comma separated list of StorageType to clear. </summary>
            public string storageTypes;
        }
        public class GetCookiesParameters
        {
            
            /// <summary> [Optional] Browser context to use when called on the browser endpoint. </summary>
            public string browserContextId;
        }
        public class SetCookiesParameters
        {
            
            /// <summary> [Require] Cookies to be set. </summary>
            public object[] cookies;
            /// <summary> [Optional] Browser context to use when called on the browser endpoint. </summary>
            public string browserContextId;
        }
        public class ClearCookiesParameters
        {
            
            /// <summary> [Optional] Browser context to use when called on the browser endpoint. </summary>
            public string browserContextId;
        }
        public class GetUsageAndQuotaParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string origin;
        }
        public class OverrideQuotaForOriginParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string origin;
            /// <summary> [Optional] The quota size (in bytes) to override the original quota with.If this is called multiple times, the overridden quota will be equal tothe quotaSize provided in the final call. If this is called withoutspecifying a quotaSize, the quota will be reset to the default value forthe specified origin. If this is called multiple times with differentorigins, the override will be maintained for each origin until it isdisabled (called without a quotaSize). </summary>
            public double quotaSize;
        }
        public class TrackCacheStorageForOriginParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string origin;
        }
        public class TrackIndexedDBForOriginParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string origin;
        }
        public class UntrackCacheStorageForOriginParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string origin;
        }
        public class UntrackIndexedDBForOriginParameters
        {
            
            /// <summary> [Require] Security origin. </summary>
            public string origin;
        }
        public class ClearTrustTokensParameters
        {
            
            /// <summary> [Require]  </summary>
            public string issuerOrigin;
        }
        public class GetInterestGroupDetailsParameters
        {
            
            /// <summary> [Require]  </summary>
            public string ownerOrigin;
            /// <summary> [Require]  </summary>
            public string name;
        }
        public class SetInterestGroupTrackingParameters
        {
            
            /// <summary> [Require]  </summary>
            public bool enable;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetCookiesReturn
        {
            
            /// <summary> Array of cookie objects. </summary>
            public object[] cookies;
        }
        public class GetUsageAndQuotaReturn
        {
            
            /// <summary> Storage usage (bytes). </summary>
            public double usage;
            /// <summary> Storage quota (bytes). </summary>
            public double quota;
            /// <summary> Whether or not the origin has an active storage quota override </summary>
            public bool overrideActive;
            /// <summary> Storage usage per type (bytes). </summary>
            public object[] usageBreakdown;
        }
        public class GetTrustTokensReturn
        {
            
            /// <summary>  </summary>
            public object[] tokens;
        }
        public class ClearTrustTokensReturn
        {
            
            /// <summary> True if any tokens were deleted, false otherwise. </summary>
            public bool didDeleteTokens;
        }
        public class GetInterestGroupDetailsReturn
        {
            
            /// <summary>  </summary>
            public Storage.InterestGroupDetailsType details;
        }
    }
    
    public class SystemInfo : DomainBase
    {
        public SystemInfo(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Returns information about the system. 
        /// </summary>
        public async Task<GetInfoReturn> GetInfo(string sessionId = default)
        {
            var ___r = await this.chrome.Send("SystemInfo.getInfo", null, sessionId);
            return Convert<GetInfoReturn>(___r);
        }
        /// <summary> 
        /// Returns information about all running processes. 
        /// </summary>
        public async Task<GetProcessInfoReturn> GetProcessInfo(string sessionId = default)
        {
            var ___r = await this.chrome.Send("SystemInfo.getProcessInfo", null, sessionId);
            return Convert<GetProcessInfoReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class GPUDeviceType
        {
            
            /// <summary> PCI ID of the GPU vendor, if available; 0 otherwise. </summary>
            public double vendorId;
            /// <summary> PCI ID of the GPU device, if available; 0 otherwise. </summary>
            public double deviceId;
            /// <summary> Sub sys ID of the GPU, only available on Windows. </summary>
            public double subSysId;
            /// <summary> Revision of the GPU, only available on Windows. </summary>
            public double revision;
            /// <summary> String description of the GPU vendor, if the PCI ID is not available. </summary>
            public string vendorString;
            /// <summary> String description of the GPU device, if the PCI ID is not available. </summary>
            public string deviceString;
            /// <summary> String description of the GPU driver vendor. </summary>
            public string driverVendor;
            /// <summary> String description of the GPU driver version. </summary>
            public string driverVersion;
        }
        public class SizeType
        {
            
            /// <summary> Width in pixels. </summary>
            public int width;
            /// <summary> Height in pixels. </summary>
            public int height;
        }
        public class VideoDecodeAcceleratorCapabilityType
        {
            
            /// <summary> Video codec profile that is supported, e.g. VP9 Profile 2. </summary>
            public string profile;
            /// <summary> Maximum video dimensions in pixels supported for this |profile|. </summary>
            public SystemInfo.SizeType maxResolution;
            /// <summary> Minimum video dimensions in pixels supported for this |profile|. </summary>
            public SystemInfo.SizeType minResolution;
        }
        public class VideoEncodeAcceleratorCapabilityType
        {
            
            /// <summary> Video codec profile that is supported, e.g H264 Main. </summary>
            public string profile;
            /// <summary> Maximum video dimensions in pixels supported for this |profile|. </summary>
            public SystemInfo.SizeType maxResolution;
            /// <summary> Maximum encoding framerate in frames per second supported for this|profile|, as fraction's numerator and denominator, e.g. 24/1 fps,24000/1001 fps, etc. </summary>
            public int maxFramerateNumerator;
            /// <summary>  </summary>
            public int maxFramerateDenominator;
        }
        public class ImageDecodeAcceleratorCapabilityType
        {
            
            /// <summary> Image coded, e.g. Jpeg. </summary>
            public string imageType;
            /// <summary> Maximum supported dimensions of the image in pixels. </summary>
            public SystemInfo.SizeType maxDimensions;
            /// <summary> Minimum supported dimensions of the image in pixels. </summary>
            public SystemInfo.SizeType minDimensions;
            /// <summary> Optional array of supported subsampling formats, e.g. 4:2:0, if known. </summary>
            public object[] subsamplings;
        }
        public class GPUInfoType
        {
            
            /// <summary> The graphics devices on the system. Element 0 is the primary GPU. </summary>
            public object[] devices;
            /// <summary> An optional dictionary of additional GPU related attributes. </summary>
            public object auxAttributes;
            /// <summary> An optional dictionary of graphics features and their status. </summary>
            public object featureStatus;
            /// <summary> An optional array of GPU driver bug workarounds. </summary>
            public object[] driverBugWorkarounds;
            /// <summary> Supported accelerated video decoding capabilities. </summary>
            public object[] videoDecoding;
            /// <summary> Supported accelerated video encoding capabilities. </summary>
            public object[] videoEncoding;
            /// <summary> Supported accelerated image decoding capabilities. </summary>
            public object[] imageDecoding;
        }
        public class ProcessInfoType
        {
            
            /// <summary> Specifies process type. </summary>
            public string type;
            /// <summary> Specifies process id. </summary>
            public int id;
            /// <summary> Specifies cumulative CPU usage in seconds across all threads of theprocess since the process start. </summary>
            public double cpuTime;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetInfoReturn
        {
            
            /// <summary> Information about the GPUs on the system. </summary>
            public SystemInfo.GPUInfoType gpu;
            /// <summary> A platform-dependent description of the model of the machine. On Mac OS, this is, forexample, 'MacBookPro'. Will be the empty string if not supported. </summary>
            public string modelName;
            /// <summary> A platform-dependent description of the version of the machine. On Mac OS, this is, forexample, '10.1'. Will be the empty string if not supported. </summary>
            public string modelVersion;
            /// <summary> The command line string used to launch the browser. Will be the empty string if notsupported. </summary>
            public string commandLine;
        }
        public class GetProcessInfoReturn
        {
            
            /// <summary> An array of process info blocks. </summary>
            public object[] processInfo;
        }
    }
    
    public class Target : DomainBase
    {
        public Target(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Issued when attached to target because of auto-attach or `attachToTarget` command. </summary>
        /// <returns> remove handler </returns>
        public Action OnAttachedToTarget(Action<OnAttachedToTargetParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAttachedToTargetParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Target.attachedToTarget" : $"Target.attachedToTarget.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when detached from target for any reason (including `detachFromTarget` command). Can beissued multiple times per target if multiple sessions have been attached to it. </summary>
        /// <returns> remove handler </returns>
        public Action OnDetachedFromTarget(Action<OnDetachedFromTargetParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDetachedFromTargetParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Target.detachedFromTarget" : $"Target.detachedFromTarget.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies about a new protocol message received from the session (as reported in`attachedToTarget` event). </summary>
        /// <returns> remove handler </returns>
        public Action OnReceivedMessageFromTarget(Action<OnReceivedMessageFromTargetParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnReceivedMessageFromTargetParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Target.receivedMessageFromTarget" : $"Target.receivedMessageFromTarget.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when a possible inspection target is created. </summary>
        /// <returns> remove handler </returns>
        public Action OnTargetCreated(Action<OnTargetCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnTargetCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Target.targetCreated" : $"Target.targetCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when a target is destroyed. </summary>
        /// <returns> remove handler </returns>
        public Action OnTargetDestroyed(Action<OnTargetDestroyedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnTargetDestroyedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Target.targetDestroyed" : $"Target.targetDestroyed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when a target has crashed. </summary>
        /// <returns> remove handler </returns>
        public Action OnTargetCrashed(Action<OnTargetCrashedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnTargetCrashedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Target.targetCrashed" : $"Target.targetCrashed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when some information about a target has changed. This only happens between`targetCreated` and `targetDestroyed`. </summary>
        /// <returns> remove handler </returns>
        public Action OnTargetInfoChanged(Action<OnTargetInfoChangedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnTargetInfoChangedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Target.targetInfoChanged" : $"Target.targetInfoChanged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Activates (focuses) the target. 
        /// </summary>
        public async Task ActivateTarget(ActivateTargetParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.activateTarget", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Attaches to the target with given id. 
        /// </summary>
        public async Task<AttachToTargetReturn> AttachToTarget(AttachToTargetParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.attachToTarget", parameters, sessionId);
            return Convert<AttachToTargetReturn>(___r);
        }
        /// <summary> 
        /// Attaches to the browser target, only uses flat sessionId mode. 
        /// </summary>
        public async Task<AttachToBrowserTargetReturn> AttachToBrowserTarget(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.attachToBrowserTarget", null, sessionId);
            return Convert<AttachToBrowserTargetReturn>(___r);
        }
        /// <summary> 
        /// Closes the target. If the target is a page that gets closed too. 
        /// </summary>
        public async Task<CloseTargetReturn> CloseTarget(CloseTargetParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.closeTarget", parameters, sessionId);
            return Convert<CloseTargetReturn>(___r);
        }
        /// <summary> 
        /// Inject object to the target's main frame that provides a communicationchannel with browser target.Injected object will be available as `window[bindingName]`.The object has the follwing API:- `binding.send(json)` - a method to send messages over the remote debugging protocol- `binding.onmessage = json => handleMessage(json)` - a callback that will be called for the protocol notifications and command responses. 
        /// </summary>
        public async Task ExposeDevToolsProtocol(ExposeDevToolsProtocolParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.exposeDevToolsProtocol", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Creates a new empty BrowserContext. Similar to an incognito profile but you can have more thanone. 
        /// </summary>
        public async Task<CreateBrowserContextReturn> CreateBrowserContext(CreateBrowserContextParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.createBrowserContext", parameters, sessionId);
            return Convert<CreateBrowserContextReturn>(___r);
        }
        /// <summary> 
        /// Returns all browser contexts created with `Target.createBrowserContext` method. 
        /// </summary>
        public async Task<GetBrowserContextsReturn> GetBrowserContexts(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.getBrowserContexts", null, sessionId);
            return Convert<GetBrowserContextsReturn>(___r);
        }
        /// <summary> 
        /// Creates a new page. 
        /// </summary>
        public async Task<CreateTargetReturn> CreateTarget(CreateTargetParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.createTarget", parameters, sessionId);
            return Convert<CreateTargetReturn>(___r);
        }
        /// <summary> 
        /// Detaches session with given id. 
        /// </summary>
        public async Task DetachFromTarget(DetachFromTargetParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.detachFromTarget", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Deletes a BrowserContext. All the belonging pages will be closed without calling theirbeforeunload hooks. 
        /// </summary>
        public async Task DisposeBrowserContext(DisposeBrowserContextParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.disposeBrowserContext", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns information about a target. 
        /// </summary>
        public async Task<GetTargetInfoReturn> GetTargetInfo(GetTargetInfoParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.getTargetInfo", parameters, sessionId);
            return Convert<GetTargetInfoReturn>(___r);
        }
        /// <summary> 
        /// Retrieves a list of available targets. 
        /// </summary>
        public async Task<GetTargetsReturn> GetTargets(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.getTargets", null, sessionId);
            return Convert<GetTargetsReturn>(___r);
        }
        /// <summary> 
        /// Sends protocol message over session with given id.Consider using flat mode instead; see commands attachToTarget, setAutoAttach,and crbug.com/991325. 
        /// </summary>
        public async Task SendMessageToTarget(SendMessageToTargetParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.sendMessageToTarget", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Controls whether to automatically attach to new targets which are considered to be related tothis one. When turned on, attaches to all existing related targets as well. When turned off,automatically detaches from all currently attached targets.This also clears all targets added by `autoAttachRelated` from the list of targets to watchfor creation of related targets. 
        /// </summary>
        public async Task SetAutoAttach(SetAutoAttachParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.setAutoAttach", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Adds the specified target to the list of targets that will be monitored for any related targetcreation (such as child frames, child workers and new versions of service worker) and reportedthrough `attachedToTarget`. The specified target is also auto-attached.This cancels the effect of any previous `setAutoAttach` and is also cancelled by subsequent`setAutoAttach`. Only available at the Browser target. 
        /// </summary>
        public async Task AutoAttachRelated(AutoAttachRelatedParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.autoAttachRelated", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Controls whether to discover available targets and notify via`targetCreated/targetInfoChanged/targetDestroyed` events. 
        /// </summary>
        public async Task SetDiscoverTargets(SetDiscoverTargetsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.setDiscoverTargets", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables target discovery for the specified locations, when `setDiscoverTargets` was set to`true`. 
        /// </summary>
        public async Task SetRemoteLocations(SetRemoteLocationsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Target.setRemoteLocations", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class TargetInfoType
        {
            
            /// <summary>  </summary>
            public string targetId;
            /// <summary>  </summary>
            public string type;
            /// <summary>  </summary>
            public string title;
            /// <summary>  </summary>
            public string url;
            /// <summary> Whether the target has an attached client. </summary>
            public bool attached;
            /// <summary> Opener target Id </summary>
            public string openerId;
            /// <summary> Whether the target has access to the originating window. </summary>
            public bool canAccessOpener;
            /// <summary> Frame id of originating window (is only set if target has an opener). </summary>
            public string openerFrameId;
            /// <summary>  </summary>
            public string browserContextId;
        }
        public class RemoteLocationType
        {
            
            /// <summary>  </summary>
            public string host;
            /// <summary>  </summary>
            public int port;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnAttachedToTargetParameters
        {
            
            /// <summary> [Require] Identifier assigned to the session used to send/receive messages. </summary>
            public string sessionId;
            /// <summary> [Require]  </summary>
            public Target.TargetInfoType targetInfo;
            /// <summary> [Require]  </summary>
            public bool waitingForDebugger;
        }
        public class OnDetachedFromTargetParameters
        {
            
            /// <summary> [Require] Detached session identifier. </summary>
            public string sessionId;
            /// <summary> [Optional] Deprecated. </summary>
            public string targetId;
        }
        public class OnReceivedMessageFromTargetParameters
        {
            
            /// <summary> [Require] Identifier of a session which sends a message. </summary>
            public string sessionId;
            /// <summary> [Require]  </summary>
            public string message;
            /// <summary> [Optional] Deprecated. </summary>
            public string targetId;
        }
        public class OnTargetCreatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public Target.TargetInfoType targetInfo;
        }
        public class OnTargetDestroyedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string targetId;
        }
        public class OnTargetCrashedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string targetId;
            /// <summary> [Require] Termination status type. </summary>
            public string status;
            /// <summary> [Require] Termination error code. </summary>
            public int errorCode;
        }
        public class OnTargetInfoChangedParameters
        {
            
            /// <summary> [Require]  </summary>
            public Target.TargetInfoType targetInfo;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class ActivateTargetParameters
        {
            
            /// <summary> [Require]  </summary>
            public string targetId;
        }
        public class AttachToTargetParameters
        {
            
            /// <summary> [Require]  </summary>
            public string targetId;
            /// <summary> [Optional] Enables "flat" access to the session via specifying sessionId attribute in the commands.We plan to make this the default, deprecate non-flattened mode,and eventually retire it. See crbug.com/991325. </summary>
            public bool flatten;
        }
        public class CloseTargetParameters
        {
            
            /// <summary> [Require]  </summary>
            public string targetId;
        }
        public class ExposeDevToolsProtocolParameters
        {
            
            /// <summary> [Require]  </summary>
            public string targetId;
            /// <summary> [Optional] Binding name, 'cdp' if not specified. </summary>
            public string bindingName;
        }
        public class CreateBrowserContextParameters
        {
            
            /// <summary> [Optional] If specified, disposes this context when debugging session disconnects. </summary>
            public bool disposeOnDetach;
            /// <summary> [Optional] Proxy server, similar to the one passed to --proxy-server </summary>
            public string proxyServer;
            /// <summary> [Optional] Proxy bypass list, similar to the one passed to --proxy-bypass-list </summary>
            public string proxyBypassList;
            /// <summary> [Optional] An optional list of origins to grant unlimited cross-origin access to.Parts of the URL other than those constituting origin are ignored. </summary>
            public object[] originsWithUniversalNetworkAccess;
        }
        public class CreateTargetParameters
        {
            
            /// <summary> [Require] The initial URL the page will be navigated to. An empty string indicates about:blank. </summary>
            public string url;
            /// <summary> [Optional] Frame width in DIP (headless chrome only). </summary>
            public int width;
            /// <summary> [Optional] Frame height in DIP (headless chrome only). </summary>
            public int height;
            /// <summary> [Optional] The browser context to create the page in. </summary>
            public string browserContextId;
            /// <summary> [Optional] Whether BeginFrames for this target will be controlled via DevTools (headless chrome only,not supported on MacOS yet, false by default). </summary>
            public bool enableBeginFrameControl;
            /// <summary> [Optional] Whether to create a new Window or Tab (chrome-only, false by default). </summary>
            public bool newWindow;
            /// <summary> [Optional] Whether to create the target in background or foreground (chrome-only,false by default). </summary>
            public bool background;
        }
        public class DetachFromTargetParameters
        {
            
            /// <summary> [Optional] Session to detach. </summary>
            public string sessionId;
            /// <summary> [Optional] Deprecated. </summary>
            public string targetId;
        }
        public class DisposeBrowserContextParameters
        {
            
            /// <summary> [Require]  </summary>
            public string browserContextId;
        }
        public class GetTargetInfoParameters
        {
            
            /// <summary> [Optional]  </summary>
            public string targetId;
        }
        public class SendMessageToTargetParameters
        {
            
            /// <summary> [Require]  </summary>
            public string message;
            /// <summary> [Optional] Identifier of the session. </summary>
            public string sessionId;
            /// <summary> [Optional] Deprecated. </summary>
            public string targetId;
        }
        public class SetAutoAttachParameters
        {
            
            /// <summary> [Require] Whether to auto-attach to related targets. </summary>
            public bool autoAttach;
            /// <summary> [Require] Whether to pause new targets when attaching to them. Use `Runtime.runIfWaitingForDebugger`to run paused targets. </summary>
            public bool waitForDebuggerOnStart;
            /// <summary> [Optional] Enables "flat" access to the session via specifying sessionId attribute in the commands.We plan to make this the default, deprecate non-flattened mode,and eventually retire it. See crbug.com/991325. </summary>
            public bool flatten;
        }
        public class AutoAttachRelatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string targetId;
            /// <summary> [Require] Whether to pause new targets when attaching to them. Use `Runtime.runIfWaitingForDebugger`to run paused targets. </summary>
            public bool waitForDebuggerOnStart;
        }
        public class SetDiscoverTargetsParameters
        {
            
            /// <summary> [Require] Whether to discover available targets. </summary>
            public bool discover;
        }
        public class SetRemoteLocationsParameters
        {
            
            /// <summary> [Require] List of remote locations. </summary>
            public object[] locations;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class AttachToTargetReturn
        {
            
            /// <summary> Id assigned to the session. </summary>
            public string sessionId;
        }
        public class AttachToBrowserTargetReturn
        {
            
            /// <summary> Id assigned to the session. </summary>
            public string sessionId;
        }
        public class CloseTargetReturn
        {
            
            /// <summary> Always set to true. If an error occurs, the response indicates protocol error. </summary>
            public bool success;
        }
        public class CreateBrowserContextReturn
        {
            
            /// <summary> The id of the context created. </summary>
            public string browserContextId;
        }
        public class GetBrowserContextsReturn
        {
            
            /// <summary> An array of browser context ids. </summary>
            public object[] browserContextIds;
        }
        public class CreateTargetReturn
        {
            
            /// <summary> The id of the page opened. </summary>
            public string targetId;
        }
        public class GetTargetInfoReturn
        {
            
            /// <summary>  </summary>
            public Target.TargetInfoType targetInfo;
        }
        public class GetTargetsReturn
        {
            
            /// <summary> The list of targets. </summary>
            public object[] targetInfos;
        }
    }
    
    public class Tethering : DomainBase
    {
        public Tethering(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Informs that port was successfully bound and got a specified connection id. </summary>
        /// <returns> remove handler </returns>
        public Action OnAccepted(Action<OnAcceptedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAcceptedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Tethering.accepted" : $"Tethering.accepted.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Request browser port binding. 
        /// </summary>
        public async Task Bind(BindParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Tethering.bind", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Request browser port unbinding. 
        /// </summary>
        public async Task Unbind(UnbindParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Tethering.unbind", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnAcceptedParameters
        {
            
            /// <summary> [Require] Port number that was successfully bound. </summary>
            public int port;
            /// <summary> [Require] Connection id to be used. </summary>
            public string connectionId;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class BindParameters
        {
            
            /// <summary> [Require] Port number to bind. </summary>
            public int port;
        }
        public class UnbindParameters
        {
            
            /// <summary> [Require] Port number to unbind. </summary>
            public int port;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class Tracing : DomainBase
    {
        public Tracing(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnBufferUsage(Action<OnBufferUsageParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnBufferUsageParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Tracing.bufferUsage" : $"Tracing.bufferUsage.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Contains an bucket of collected trace events. When tracing is stopped collected events will besend as a sequence of dataCollected events followed by tracingComplete event. </summary>
        /// <returns> remove handler </returns>
        public Action OnDataCollected(Action<OnDataCollectedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnDataCollectedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Tracing.dataCollected" : $"Tracing.dataCollected.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Signals that tracing is stopped and there is no trace buffers pending flush, all data weredelivered via dataCollected events. </summary>
        /// <returns> remove handler </returns>
        public Action OnTracingComplete(Action<OnTracingCompleteParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnTracingCompleteParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Tracing.tracingComplete" : $"Tracing.tracingComplete.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Stop trace events collection. 
        /// </summary>
        public async Task End(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Tracing.end", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Gets supported tracing categories. 
        /// </summary>
        public async Task<GetCategoriesReturn> GetCategories(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Tracing.getCategories", null, sessionId);
            return Convert<GetCategoriesReturn>(___r);
        }
        /// <summary> 
        /// Record a clock sync marker in the trace. 
        /// </summary>
        public async Task RecordClockSyncMarker(RecordClockSyncMarkerParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Tracing.recordClockSyncMarker", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Request a global memory dump. 
        /// </summary>
        public async Task<RequestMemoryDumpReturn> RequestMemoryDump(RequestMemoryDumpParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Tracing.requestMemoryDump", parameters, sessionId);
            return Convert<RequestMemoryDumpReturn>(___r);
        }
        /// <summary> 
        /// Start trace events collection. 
        /// </summary>
        public async Task Start(StartParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Tracing.start", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class MemoryDumpConfigType
        {
            
        }
        public class TraceConfigType
        {
            
            /// <summary> Controls how the trace buffer stores data. </summary>
            public string recordMode;
            /// <summary> Turns on JavaScript stack sampling. </summary>
            public bool enableSampling;
            /// <summary> Turns on system tracing. </summary>
            public bool enableSystrace;
            /// <summary> Turns on argument filter. </summary>
            public bool enableArgumentFilter;
            /// <summary> Included category filters. </summary>
            public object[] includedCategories;
            /// <summary> Excluded category filters. </summary>
            public object[] excludedCategories;
            /// <summary> Configuration to synthesize the delays in tracing. </summary>
            public object[] syntheticDelays;
            /// <summary> Configuration for memory dump triggers. Used only when "memory-infra" category is enabled. </summary>
            public Tracing.MemoryDumpConfigType memoryDumpConfig;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnBufferUsageParameters
        {
            
            /// <summary> [Optional] A number in range [0..1] that indicates the used size of event buffer as a fraction of itstotal size. </summary>
            public double percentFull;
            /// <summary> [Optional] An approximate number of events in the trace log. </summary>
            public double eventCount;
            /// <summary> [Optional] A number in range [0..1] that indicates the used size of event buffer as a fraction of itstotal size. </summary>
            public double value;
        }
        public class OnDataCollectedParameters
        {
            
            /// <summary> [Require]  </summary>
            public object[] value;
        }
        public class OnTracingCompleteParameters
        {
            
            /// <summary> [Require] Indicates whether some trace data is known to have been lost, e.g. because the trace ringbuffer wrapped around. </summary>
            public bool dataLossOccurred;
            /// <summary> [Optional] A handle of the stream that holds resulting trace data. </summary>
            public string stream;
            /// <summary> [Optional] Trace data format of returned stream. </summary>
            public string traceFormat;
            /// <summary> [Optional] Compression format of returned stream. </summary>
            public string streamCompression;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class RecordClockSyncMarkerParameters
        {
            
            /// <summary> [Require] The ID of this clock sync marker </summary>
            public string syncId;
        }
        public class RequestMemoryDumpParameters
        {
            
            /// <summary> [Optional] Enables more deterministic results by forcing garbage collection </summary>
            public bool deterministic;
            /// <summary> [Optional] Specifies level of details in memory dump. Defaults to "detailed". </summary>
            public string levelOfDetail;
        }
        public class StartParameters
        {
            
            /// <summary> [Optional] Category/tag filter </summary>
            public string categories;
            /// <summary> [Optional] Tracing options </summary>
            public string options;
            /// <summary> [Optional] If set, the agent will issue bufferUsage events at this interval, specified in milliseconds </summary>
            public double bufferUsageReportingInterval;
            /// <summary> [Optional] Whether to report trace events as series of dataCollected events or to save trace to astream (defaults to `ReportEvents`). </summary>
            public string transferMode;
            /// <summary> [Optional] Trace data format to use. This only applies when using `ReturnAsStream`transfer mode (defaults to `json`). </summary>
            public string streamFormat;
            /// <summary> [Optional] Compression format to use. This only applies when using `ReturnAsStream`transfer mode (defaults to `none`) </summary>
            public string streamCompression;
            /// <summary> [Optional]  </summary>
            public Tracing.TraceConfigType traceConfig;
            /// <summary> [Optional] Base64-encoded serialized perfetto.protos.TraceConfig protobuf messageWhen specified, the parameters `categories`, `options`, `traceConfig`are ignored. (Encoded as a base64 string when passed over JSON) </summary>
            public string perfettoConfig;
            /// <summary> [Optional] Backend type (defaults to `auto`) </summary>
            public string tracingBackend;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetCategoriesReturn
        {
            
            /// <summary> A list of supported tracing categories. </summary>
            public object[] categories;
        }
        public class RequestMemoryDumpReturn
        {
            
            /// <summary> GUID of the resulting global memory dump. </summary>
            public string dumpGuid;
            /// <summary> True iff the global memory dump succeeded. </summary>
            public bool success;
        }
    }
    
    public class Fetch : DomainBase
    {
        public Fetch(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Issued when the domain is enabled and the request URL matches thespecified filter. The request is paused until the client respondswith one of continueRequest, failRequest or fulfillRequest.The stage of the request can be determined by presence of responseErrorReasonand responseStatusCode -- the request is at the response stage if eitherof these fields is present and in the request stage otherwise. </summary>
        /// <returns> remove handler </returns>
        public Action OnRequestPaused(Action<OnRequestPausedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnRequestPausedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Fetch.requestPaused" : $"Fetch.requestPaused.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when the domain is enabled with handleAuthRequests set to true.The request is paused until client responds with continueWithAuth. </summary>
        /// <returns> remove handler </returns>
        public Action OnAuthRequired(Action<OnAuthRequiredParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAuthRequiredParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Fetch.authRequired" : $"Fetch.authRequired.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Disables the fetch domain. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Fetch.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables issuing of requestPaused events. A request will be paused until clientcalls one of failRequest, fulfillRequest or continueRequest/continueWithAuth. 
        /// </summary>
        public async Task Enable(EnableParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Fetch.enable", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Causes the request to fail with specified reason. 
        /// </summary>
        public async Task FailRequest(FailRequestParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Fetch.failRequest", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Provides response to the request. 
        /// </summary>
        public async Task FulfillRequest(FulfillRequestParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Fetch.fulfillRequest", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Continues the request, optionally modifying some of its parameters. 
        /// </summary>
        public async Task ContinueRequest(ContinueRequestParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Fetch.continueRequest", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Continues a request supplying authChallengeResponse following authRequired event. 
        /// </summary>
        public async Task ContinueWithAuth(ContinueWithAuthParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Fetch.continueWithAuth", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Continues loading of the paused response, optionally modifying theresponse headers. If either responseCode or headers are modified, all of themmust be present. 
        /// </summary>
        public async Task ContinueResponse(ContinueResponseParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Fetch.continueResponse", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Causes the body of the response to be received from the server andreturned as a single string. May only be issued for a request thatis paused in the Response stage and is mutually exclusive withtakeResponseBodyForInterceptionAsStream. Calling other methods thataffect the request or disabling fetch domain before body is receivedresults in an undefined behavior. 
        /// </summary>
        public async Task<GetResponseBodyReturn> GetResponseBody(GetResponseBodyParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Fetch.getResponseBody", parameters, sessionId);
            return Convert<GetResponseBodyReturn>(___r);
        }
        /// <summary> 
        /// Returns a handle to the stream representing the response body.The request must be paused in the HeadersReceived stage.Note that after this command the request can't be continuedas is -- client either needs to cancel it or to provide theresponse body.The stream only supports sequential read, IO.read will fail if the positionis specified.This method is mutually exclusive with getResponseBody.Calling other methods that affect the request or disabling fetchdomain before body is received results in an undefined behavior. 
        /// </summary>
        public async Task<TakeResponseBodyAsStreamReturn> TakeResponseBodyAsStream(TakeResponseBodyAsStreamParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Fetch.takeResponseBodyAsStream", parameters, sessionId);
            return Convert<TakeResponseBodyAsStreamReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class RequestPatternType
        {
            
            /// <summary> Wildcards (`'*'` -> zero or more, `'?'` -> exactly one) are allowed. Escape character isbackslash. Omitting is equivalent to `"*"`. </summary>
            public string urlPattern;
            /// <summary> If set, only requests for matching resource types will be intercepted. </summary>
            public string resourceType;
            /// <summary> Stage at which to begin intercepting requests. Default is Request. </summary>
            public string requestStage;
        }
        public class HeaderEntryType
        {
            
            /// <summary>  </summary>
            public string name;
            /// <summary>  </summary>
            public string value;
        }
        public class AuthChallengeType
        {
            
            /// <summary> Source of the authentication challenge. </summary>
            public string source;
            /// <summary> Origin of the challenger. </summary>
            public string origin;
            /// <summary> The authentication scheme used, such as basic or digest </summary>
            public string scheme;
            /// <summary> The realm of the challenge. May be empty. </summary>
            public string realm;
        }
        public class AuthChallengeResponseType
        {
            
            /// <summary> The decision on what to do in response to the authorization challenge.  Default meansdeferring to the default behavior of the net stack, which will likely either the Cancelauthentication or display a popup dialog box. </summary>
            public string response;
            /// <summary> The username to provide, possibly empty. Should only be set if response isProvideCredentials. </summary>
            public string username;
            /// <summary> The password to provide, possibly empty. Should only be set if response isProvideCredentials. </summary>
            public string password;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnRequestPausedParameters
        {
            
            /// <summary> [Require] Each request the page makes will have a unique id. </summary>
            public string requestId;
            /// <summary> [Require] The details of the request. </summary>
            public Network.RequestType request;
            /// <summary> [Require] The id of the frame that initiated the request. </summary>
            public string frameId;
            /// <summary> [Require] How the requested resource will be used. </summary>
            public string resourceType;
            /// <summary> [Optional] Response error if intercepted at response stage. </summary>
            public string responseErrorReason;
            /// <summary> [Optional] Response code if intercepted at response stage. </summary>
            public int responseStatusCode;
            /// <summary> [Optional] Response status text if intercepted at response stage. </summary>
            public string responseStatusText;
            /// <summary> [Optional] Response headers if intercepted at the response stage. </summary>
            public object[] responseHeaders;
            /// <summary> [Optional] If the intercepted request had a corresponding Network.requestWillBeSent event fired for it,then this networkId will be the same as the requestId present in the requestWillBeSent event. </summary>
            public string networkId;
        }
        public class OnAuthRequiredParameters
        {
            
            /// <summary> [Require] Each request the page makes will have a unique id. </summary>
            public string requestId;
            /// <summary> [Require] The details of the request. </summary>
            public Network.RequestType request;
            /// <summary> [Require] The id of the frame that initiated the request. </summary>
            public string frameId;
            /// <summary> [Require] How the requested resource will be used. </summary>
            public string resourceType;
            /// <summary> [Require] Details of the Authorization Challenge encountered.If this is set, client should respond with continueRequest thatcontains AuthChallengeResponse. </summary>
            public Fetch.AuthChallengeType authChallenge;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class EnableParameters
        {
            
            /// <summary> [Optional] If specified, only requests matching any of these patterns will producefetchRequested event and will be paused until clients response. If not set,all requests will be affected. </summary>
            public object[] patterns;
            /// <summary> [Optional] If true, authRequired events will be issued and requests will be pausedexpecting a call to continueWithAuth. </summary>
            public bool handleAuthRequests;
        }
        public class FailRequestParameters
        {
            
            /// <summary> [Require] An id the client received in requestPaused event. </summary>
            public string requestId;
            /// <summary> [Require] Causes the request to fail with the given reason. </summary>
            public string errorReason;
        }
        public class FulfillRequestParameters
        {
            
            /// <summary> [Require] An id the client received in requestPaused event. </summary>
            public string requestId;
            /// <summary> [Require] An HTTP response code. </summary>
            public int responseCode;
            /// <summary> [Optional] Response headers. </summary>
            public object[] responseHeaders;
            /// <summary> [Optional] Alternative way of specifying response headers as a \0-separatedseries of name: value pairs. Prefer the above method unless youneed to represent some non-UTF8 values that can't be transmittedover the protocol as text. (Encoded as a base64 string when passed over JSON) </summary>
            public string binaryResponseHeaders;
            /// <summary> [Optional] A response body. If absent, original response body will be used ifthe request is intercepted at the response stage and empty bodywill be used if the request is intercepted at the request stage. (Encoded as a base64 string when passed over JSON) </summary>
            public string body;
            /// <summary> [Optional] A textual representation of responseCode.If absent, a standard phrase matching responseCode is used. </summary>
            public string responsePhrase;
        }
        public class ContinueRequestParameters
        {
            
            /// <summary> [Require] An id the client received in requestPaused event. </summary>
            public string requestId;
            /// <summary> [Optional] If set, the request url will be modified in a way that's not observable by page. </summary>
            public string url;
            /// <summary> [Optional] If set, the request method is overridden. </summary>
            public string method;
            /// <summary> [Optional] If set, overrides the post data in the request. (Encoded as a base64 string when passed over JSON) </summary>
            public string postData;
            /// <summary> [Optional] If set, overrides the request headers. </summary>
            public object[] headers;
            /// <summary> [Optional] If set, overrides response interception behavior for this request. </summary>
            public bool interceptResponse;
        }
        public class ContinueWithAuthParameters
        {
            
            /// <summary> [Require] An id the client received in authRequired event. </summary>
            public string requestId;
            /// <summary> [Require] Response to  with an authChallenge. </summary>
            public Fetch.AuthChallengeResponseType authChallengeResponse;
        }
        public class ContinueResponseParameters
        {
            
            /// <summary> [Require] An id the client received in requestPaused event. </summary>
            public string requestId;
            /// <summary> [Optional] An HTTP response code. If absent, original response code will be used. </summary>
            public int responseCode;
            /// <summary> [Optional] A textual representation of responseCode.If absent, a standard phrase matching responseCode is used. </summary>
            public string responsePhrase;
            /// <summary> [Optional] Response headers. If absent, original response headers will be used. </summary>
            public object[] responseHeaders;
            /// <summary> [Optional] Alternative way of specifying response headers as a \0-separatedseries of name: value pairs. Prefer the above method unless youneed to represent some non-UTF8 values that can't be transmittedover the protocol as text. (Encoded as a base64 string when passed over JSON) </summary>
            public string binaryResponseHeaders;
        }
        public class GetResponseBodyParameters
        {
            
            /// <summary> [Require] Identifier for the intercepted request to get body for. </summary>
            public string requestId;
        }
        public class TakeResponseBodyAsStreamParameters
        {
            
            /// <summary> [Require]  </summary>
            public string requestId;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetResponseBodyReturn
        {
            
            /// <summary> Response body. </summary>
            public string body;
            /// <summary> True, if content was sent as base64. </summary>
            public bool base64Encoded;
        }
        public class TakeResponseBodyAsStreamReturn
        {
            
            /// <summary>  </summary>
            public string stream;
        }
    }
    
    public class WebAudio : DomainBase
    {
        public WebAudio(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Notifies that a new BaseAudioContext has been created. </summary>
        /// <returns> remove handler </returns>
        public Action OnContextCreated(Action<OnContextCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnContextCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.contextCreated" : $"WebAudio.contextCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that an existing BaseAudioContext will be destroyed. </summary>
        /// <returns> remove handler </returns>
        public Action OnContextWillBeDestroyed(Action<OnContextWillBeDestroyedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnContextWillBeDestroyedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.contextWillBeDestroyed" : $"WebAudio.contextWillBeDestroyed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that existing BaseAudioContext has changed some properties (id stays the same).. </summary>
        /// <returns> remove handler </returns>
        public Action OnContextChanged(Action<OnContextChangedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnContextChangedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.contextChanged" : $"WebAudio.contextChanged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that the construction of an AudioListener has finished. </summary>
        /// <returns> remove handler </returns>
        public Action OnAudioListenerCreated(Action<OnAudioListenerCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAudioListenerCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.audioListenerCreated" : $"WebAudio.audioListenerCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that a new AudioListener has been created. </summary>
        /// <returns> remove handler </returns>
        public Action OnAudioListenerWillBeDestroyed(Action<OnAudioListenerWillBeDestroyedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAudioListenerWillBeDestroyedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.audioListenerWillBeDestroyed" : $"WebAudio.audioListenerWillBeDestroyed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that a new AudioNode has been created. </summary>
        /// <returns> remove handler </returns>
        public Action OnAudioNodeCreated(Action<OnAudioNodeCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAudioNodeCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.audioNodeCreated" : $"WebAudio.audioNodeCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that an existing AudioNode has been destroyed. </summary>
        /// <returns> remove handler </returns>
        public Action OnAudioNodeWillBeDestroyed(Action<OnAudioNodeWillBeDestroyedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAudioNodeWillBeDestroyedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.audioNodeWillBeDestroyed" : $"WebAudio.audioNodeWillBeDestroyed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that a new AudioParam has been created. </summary>
        /// <returns> remove handler </returns>
        public Action OnAudioParamCreated(Action<OnAudioParamCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAudioParamCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.audioParamCreated" : $"WebAudio.audioParamCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that an existing AudioParam has been destroyed. </summary>
        /// <returns> remove handler </returns>
        public Action OnAudioParamWillBeDestroyed(Action<OnAudioParamWillBeDestroyedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAudioParamWillBeDestroyedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.audioParamWillBeDestroyed" : $"WebAudio.audioParamWillBeDestroyed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that two AudioNodes are connected. </summary>
        /// <returns> remove handler </returns>
        public Action OnNodesConnected(Action<OnNodesConnectedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnNodesConnectedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.nodesConnected" : $"WebAudio.nodesConnected.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that AudioNodes are disconnected. The destination can be null, and it means all the outgoing connections from the source are disconnected. </summary>
        /// <returns> remove handler </returns>
        public Action OnNodesDisconnected(Action<OnNodesDisconnectedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnNodesDisconnectedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.nodesDisconnected" : $"WebAudio.nodesDisconnected.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that an AudioNode is connected to an AudioParam. </summary>
        /// <returns> remove handler </returns>
        public Action OnNodeParamConnected(Action<OnNodeParamConnectedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnNodeParamConnectedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.nodeParamConnected" : $"WebAudio.nodeParamConnected.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Notifies that an AudioNode is disconnected to an AudioParam. </summary>
        /// <returns> remove handler </returns>
        public Action OnNodeParamDisconnected(Action<OnNodeParamDisconnectedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnNodeParamDisconnectedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "WebAudio.nodeParamDisconnected" : $"WebAudio.nodeParamDisconnected.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Enables the WebAudio domain and starts sending context lifetime events. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAudio.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables the WebAudio domain. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAudio.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Fetch the realtime data from the registered contexts. 
        /// </summary>
        public async Task<GetRealtimeDataReturn> GetRealtimeData(GetRealtimeDataParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAudio.getRealtimeData", parameters, sessionId);
            return Convert<GetRealtimeDataReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class ContextRealtimeDataType
        {
            
            /// <summary> The current context time in second in BaseAudioContext. </summary>
            public double currentTime;
            /// <summary> The time spent on rendering graph divided by render quantum duration,and multiplied by 100. 100 means the audio renderer reached the fullcapacity and glitch may occur. </summary>
            public double renderCapacity;
            /// <summary> A running mean of callback interval. </summary>
            public double callbackIntervalMean;
            /// <summary> A running variance of callback interval. </summary>
            public double callbackIntervalVariance;
        }
        public class BaseAudioContextType
        {
            
            /// <summary>  </summary>
            public string contextId;
            /// <summary>  </summary>
            public string contextType;
            /// <summary>  </summary>
            public string contextState;
            /// <summary>  </summary>
            public WebAudio.ContextRealtimeDataType realtimeData;
            /// <summary> Platform-dependent callback buffer size. </summary>
            public double callbackBufferSize;
            /// <summary> Number of output channels supported by audio hardware in use. </summary>
            public double maxOutputChannelCount;
            /// <summary> Context sample rate. </summary>
            public double sampleRate;
        }
        public class AudioListenerType
        {
            
            /// <summary>  </summary>
            public string listenerId;
            /// <summary>  </summary>
            public string contextId;
        }
        public class AudioNodeType
        {
            
            /// <summary>  </summary>
            public string nodeId;
            /// <summary>  </summary>
            public string contextId;
            /// <summary>  </summary>
            public string nodeType;
            /// <summary>  </summary>
            public double numberOfInputs;
            /// <summary>  </summary>
            public double numberOfOutputs;
            /// <summary>  </summary>
            public double channelCount;
            /// <summary>  </summary>
            public string channelCountMode;
            /// <summary>  </summary>
            public string channelInterpretation;
        }
        public class AudioParamType
        {
            
            /// <summary>  </summary>
            public string paramId;
            /// <summary>  </summary>
            public string nodeId;
            /// <summary>  </summary>
            public string contextId;
            /// <summary>  </summary>
            public string paramType;
            /// <summary>  </summary>
            public string rate;
            /// <summary>  </summary>
            public double defaultValue;
            /// <summary>  </summary>
            public double minValue;
            /// <summary>  </summary>
            public double maxValue;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnContextCreatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public WebAudio.BaseAudioContextType context;
        }
        public class OnContextWillBeDestroyedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string contextId;
        }
        public class OnContextChangedParameters
        {
            
            /// <summary> [Require]  </summary>
            public WebAudio.BaseAudioContextType context;
        }
        public class OnAudioListenerCreatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public WebAudio.AudioListenerType listener;
        }
        public class OnAudioListenerWillBeDestroyedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string contextId;
            /// <summary> [Require]  </summary>
            public string listenerId;
        }
        public class OnAudioNodeCreatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public WebAudio.AudioNodeType node;
        }
        public class OnAudioNodeWillBeDestroyedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string contextId;
            /// <summary> [Require]  </summary>
            public string nodeId;
        }
        public class OnAudioParamCreatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public WebAudio.AudioParamType param;
        }
        public class OnAudioParamWillBeDestroyedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string contextId;
            /// <summary> [Require]  </summary>
            public string nodeId;
            /// <summary> [Require]  </summary>
            public string paramId;
        }
        public class OnNodesConnectedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string contextId;
            /// <summary> [Require]  </summary>
            public string sourceId;
            /// <summary> [Require]  </summary>
            public string destinationId;
            /// <summary> [Optional]  </summary>
            public double sourceOutputIndex;
            /// <summary> [Optional]  </summary>
            public double destinationInputIndex;
        }
        public class OnNodesDisconnectedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string contextId;
            /// <summary> [Require]  </summary>
            public string sourceId;
            /// <summary> [Require]  </summary>
            public string destinationId;
            /// <summary> [Optional]  </summary>
            public double sourceOutputIndex;
            /// <summary> [Optional]  </summary>
            public double destinationInputIndex;
        }
        public class OnNodeParamConnectedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string contextId;
            /// <summary> [Require]  </summary>
            public string sourceId;
            /// <summary> [Require]  </summary>
            public string destinationId;
            /// <summary> [Optional]  </summary>
            public double sourceOutputIndex;
        }
        public class OnNodeParamDisconnectedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string contextId;
            /// <summary> [Require]  </summary>
            public string sourceId;
            /// <summary> [Require]  </summary>
            public string destinationId;
            /// <summary> [Optional]  </summary>
            public double sourceOutputIndex;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class GetRealtimeDataParameters
        {
            
            /// <summary> [Require]  </summary>
            public string contextId;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetRealtimeDataReturn
        {
            
            /// <summary>  </summary>
            public WebAudio.ContextRealtimeDataType realtimeData;
        }
    }
    
    public class WebAuthn : DomainBase
    {
        public WebAuthn(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Enable the WebAuthn domain and start intercepting credential storage andretrieval with a virtual authenticator. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Disable the WebAuthn domain. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Creates and adds a virtual authenticator. 
        /// </summary>
        public async Task<AddVirtualAuthenticatorReturn> AddVirtualAuthenticator(AddVirtualAuthenticatorParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.addVirtualAuthenticator", parameters, sessionId);
            return Convert<AddVirtualAuthenticatorReturn>(___r);
        }
        /// <summary> 
        /// Removes the given authenticator. 
        /// </summary>
        public async Task RemoveVirtualAuthenticator(RemoveVirtualAuthenticatorParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.removeVirtualAuthenticator", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Adds the credential to the specified authenticator. 
        /// </summary>
        public async Task AddCredential(AddCredentialParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.addCredential", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Returns a single credential stored in the given virtual authenticator thatmatches the credential ID. 
        /// </summary>
        public async Task<GetCredentialReturn> GetCredential(GetCredentialParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.getCredential", parameters, sessionId);
            return Convert<GetCredentialReturn>(___r);
        }
        /// <summary> 
        /// Returns all the credentials stored in the given virtual authenticator. 
        /// </summary>
        public async Task<GetCredentialsReturn> GetCredentials(GetCredentialsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.getCredentials", parameters, sessionId);
            return Convert<GetCredentialsReturn>(___r);
        }
        /// <summary> 
        /// Removes a credential from the authenticator. 
        /// </summary>
        public async Task RemoveCredential(RemoveCredentialParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.removeCredential", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Clears all the credentials from the specified device. 
        /// </summary>
        public async Task ClearCredentials(ClearCredentialsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.clearCredentials", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets whether User Verification succeeds or fails for an authenticator.The default is true. 
        /// </summary>
        public async Task SetUserVerified(SetUserVerifiedParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.setUserVerified", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets whether tests of user presence will succeed immediately (if true) or fail to resolve (if false) for an authenticator.The default is true. 
        /// </summary>
        public async Task SetAutomaticPresenceSimulation(SetAutomaticPresenceSimulationParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("WebAuthn.setAutomaticPresenceSimulation", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class VirtualAuthenticatorOptionsType
        {
            
            /// <summary>  </summary>
            public string protocol;
            /// <summary> Defaults to ctap2_0. Ignored if |protocol| == u2f. </summary>
            public string ctap2Version;
            /// <summary>  </summary>
            public string transport;
            /// <summary> Defaults to false. </summary>
            public bool hasResidentKey;
            /// <summary> Defaults to false. </summary>
            public bool hasUserVerification;
            /// <summary> If set to true, the authenticator will support the largeBlob extension.https://w3c.github.io/webauthn#largeBlobDefaults to false. </summary>
            public bool hasLargeBlob;
            /// <summary> If set to true, the authenticator will support the credBlob extension.https://fidoalliance.org/specs/fido-v2.1-rd-20201208/fido-client-to-authenticator-protocol-v2.1-rd-20201208.html#sctn-credBlob-extensionDefaults to false. </summary>
            public bool hasCredBlob;
            /// <summary> If set to true, the authenticator will support the minPinLength extension.https://fidoalliance.org/specs/fido-v2.1-ps-20210615/fido-client-to-authenticator-protocol-v2.1-ps-20210615.html#sctn-minpinlength-extensionDefaults to false. </summary>
            public bool hasMinPinLength;
            /// <summary> If set to true, tests of user presence will succeed immediately.Otherwise, they will not be resolved. Defaults to true. </summary>
            public bool automaticPresenceSimulation;
            /// <summary> Sets whether User Verification succeeds or fails for an authenticator.Defaults to false. </summary>
            public bool isUserVerified;
        }
        public class CredentialType
        {
            
            /// <summary>  </summary>
            public string credentialId;
            /// <summary>  </summary>
            public bool isResidentCredential;
            /// <summary> Relying Party ID the credential is scoped to. Must be set when adding acredential. </summary>
            public string rpId;
            /// <summary> The ECDSA P-256 private key in PKCS#8 format. (Encoded as a base64 string when passed over JSON) </summary>
            public string privateKey;
            /// <summary> An opaque byte sequence with a maximum size of 64 bytes mapping thecredential to a specific user. (Encoded as a base64 string when passed over JSON) </summary>
            public string userHandle;
            /// <summary> Signature counter. This is incremented by one for each successfulassertion.See https://w3c.github.io/webauthn/#signature-counter </summary>
            public int signCount;
            /// <summary> The large blob associated with the credential.See https://w3c.github.io/webauthn/#sctn-large-blob-extension (Encoded as a base64 string when passed over JSON) </summary>
            public string largeBlob;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class AddVirtualAuthenticatorParameters
        {
            
            /// <summary> [Require]  </summary>
            public WebAuthn.VirtualAuthenticatorOptionsType options;
        }
        public class RemoveVirtualAuthenticatorParameters
        {
            
            /// <summary> [Require]  </summary>
            public string authenticatorId;
        }
        public class AddCredentialParameters
        {
            
            /// <summary> [Require]  </summary>
            public string authenticatorId;
            /// <summary> [Require]  </summary>
            public WebAuthn.CredentialType credential;
        }
        public class GetCredentialParameters
        {
            
            /// <summary> [Require]  </summary>
            public string authenticatorId;
            /// <summary> [Require]  </summary>
            public string credentialId;
        }
        public class GetCredentialsParameters
        {
            
            /// <summary> [Require]  </summary>
            public string authenticatorId;
        }
        public class RemoveCredentialParameters
        {
            
            /// <summary> [Require]  </summary>
            public string authenticatorId;
            /// <summary> [Require]  </summary>
            public string credentialId;
        }
        public class ClearCredentialsParameters
        {
            
            /// <summary> [Require]  </summary>
            public string authenticatorId;
        }
        public class SetUserVerifiedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string authenticatorId;
            /// <summary> [Require]  </summary>
            public bool isUserVerified;
        }
        public class SetAutomaticPresenceSimulationParameters
        {
            
            /// <summary> [Require]  </summary>
            public string authenticatorId;
            /// <summary> [Require]  </summary>
            public bool enabled;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class AddVirtualAuthenticatorReturn
        {
            
            /// <summary>  </summary>
            public string authenticatorId;
        }
        public class GetCredentialReturn
        {
            
            /// <summary>  </summary>
            public WebAuthn.CredentialType credential;
        }
        public class GetCredentialsReturn
        {
            
            /// <summary>  </summary>
            public object[] credentials;
        }
    }
    
    public class Media : DomainBase
    {
        public Media(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> This can be called multiple times, and can be used to set / override /remove player properties. A null propValue indicates removal. </summary>
        /// <returns> remove handler </returns>
        public Action OnPlayerPropertiesChanged(Action<OnPlayerPropertiesChangedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnPlayerPropertiesChangedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Media.playerPropertiesChanged" : $"Media.playerPropertiesChanged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Send events as a list, allowing them to be batched on the browser for lesscongestion. If batched, events must ALWAYS be in chronological order. </summary>
        /// <returns> remove handler </returns>
        public Action OnPlayerEventsAdded(Action<OnPlayerEventsAddedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnPlayerEventsAddedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Media.playerEventsAdded" : $"Media.playerEventsAdded.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Send a list of any messages that need to be delivered. </summary>
        /// <returns> remove handler </returns>
        public Action OnPlayerMessagesLogged(Action<OnPlayerMessagesLoggedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnPlayerMessagesLoggedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Media.playerMessagesLogged" : $"Media.playerMessagesLogged.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Send a list of any errors that need to be delivered. </summary>
        /// <returns> remove handler </returns>
        public Action OnPlayerErrorsRaised(Action<OnPlayerErrorsRaisedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnPlayerErrorsRaisedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Media.playerErrorsRaised" : $"Media.playerErrorsRaised.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Called whenever a player is created, or when a new agent joins and receivesa list of active players. If an agent is restored, it will receive the fulllist of player ids and all events again. </summary>
        /// <returns> remove handler </returns>
        public Action OnPlayersCreated(Action<OnPlayersCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnPlayersCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Media.playersCreated" : $"Media.playersCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Enables the Media domain 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Media.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables the Media domain. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Media.disable", null, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class PlayerMessageType
        {
            
            /// <summary> Keep in sync with MediaLogMessageLevelWe are currently keeping the message level 'error' separate from thePlayerError type because right now they represent different things,this one being a DVLOG(ERROR) style log message that gets printedbased on what log level is selected in the UI, and the other is arepresentation of a media::PipelineStatus object. Soon however we'regoing to be moving away from using PipelineStatus for errors andintroducing a new error type which should hopefully let us integratethe error log level into the PlayerError type. </summary>
            public string level;
            /// <summary>  </summary>
            public string message;
        }
        public class PlayerPropertyType
        {
            
            /// <summary>  </summary>
            public string name;
            /// <summary>  </summary>
            public string value;
        }
        public class PlayerEventType
        {
            
            /// <summary>  </summary>
            public double timestamp;
            /// <summary>  </summary>
            public string value;
        }
        public class PlayerErrorType
        {
            
            /// <summary>  </summary>
            public string type;
            /// <summary> When this switches to using media::Status instead of PipelineStatuswe can remove "errorCode" and replace it with the fields froma Status instance. This also seems like a duplicate of the errorlevel enum - there is a todo bug to have that level removed anduse this instead. (crbug.com/1068454) </summary>
            public string errorCode;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnPlayerPropertiesChangedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string playerId;
            /// <summary> [Require]  </summary>
            public object[] properties;
        }
        public class OnPlayerEventsAddedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string playerId;
            /// <summary> [Require]  </summary>
            public object[] events;
        }
        public class OnPlayerMessagesLoggedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string playerId;
            /// <summary> [Require]  </summary>
            public object[] messages;
        }
        public class OnPlayerErrorsRaisedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string playerId;
            /// <summary> [Require]  </summary>
            public object[] errors;
        }
        public class OnPlayersCreatedParameters
        {
            
            /// <summary> [Require]  </summary>
            public object[] players;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class Console : DomainBase
    {
        public Console(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Issued when new console message is added. </summary>
        /// <returns> remove handler </returns>
        public Action OnMessageAdded(Action<OnMessageAddedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnMessageAddedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Console.messageAdded" : $"Console.messageAdded.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Does nothing. 
        /// </summary>
        public async Task ClearMessages(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Console.clearMessages", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables console domain, prevents further console messages from being reported to the client. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Console.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables console domain, sends the messages collected so far to the client by means of the`messageAdded` notification. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Console.enable", null, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class ConsoleMessageType
        {
            
            /// <summary> Message source. </summary>
            public string source;
            /// <summary> Message severity. </summary>
            public string level;
            /// <summary> Message text. </summary>
            public string text;
            /// <summary> URL of the message origin. </summary>
            public string url;
            /// <summary> Line number in the resource that generated this message (1-based). </summary>
            public int line;
            /// <summary> Column number in the resource that generated this message (1-based). </summary>
            public int column;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnMessageAddedParameters
        {
            
            /// <summary> [Require] Console message that has been added. </summary>
            public Console.ConsoleMessageType message;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
    }
    
    public class Debugger : DomainBase
    {
        public Debugger(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Fired when breakpoint is resolved to an actual script and location. </summary>
        /// <returns> remove handler </returns>
        public Action OnBreakpointResolved(Action<OnBreakpointResolvedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnBreakpointResolvedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Debugger.breakpointResolved" : $"Debugger.breakpointResolved.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when the virtual machine stopped on breakpoint or exception or any other stop criteria. </summary>
        /// <returns> remove handler </returns>
        public Action OnPaused(Action<OnPausedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnPausedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Debugger.paused" : $"Debugger.paused.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when the virtual machine resumed execution. </summary>
        /// <returns> remove handler </returns>
        public Action OnResumed(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "Debugger.resumed" : $"Debugger.resumed.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        
        /// <summary> Fired when virtual machine fails to parse the script. </summary>
        /// <returns> remove handler </returns>
        public Action OnScriptFailedToParse(Action<OnScriptFailedToParseParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnScriptFailedToParseParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Debugger.scriptFailedToParse" : $"Debugger.scriptFailedToParse.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Fired when virtual machine parses script. This event is also fired for all known and uncollectedscripts upon enabling debugger. </summary>
        /// <returns> remove handler </returns>
        public Action OnScriptParsed(Action<OnScriptParsedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnScriptParsedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Debugger.scriptParsed" : $"Debugger.scriptParsed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Continues execution until specific location is reached. 
        /// </summary>
        public async Task ContinueToLocation(ContinueToLocationParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.continueToLocation", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Disables debugger for given page. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables debugger for the given page. Clients should not assume that the debugging has beenenabled until the result for this command is received. 
        /// </summary>
        public async Task<EnableReturn> Enable(EnableParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.enable", parameters, sessionId);
            return Convert<EnableReturn>(___r);
        }
        /// <summary> 
        /// Evaluates expression on a given call frame. 
        /// </summary>
        public async Task<EvaluateOnCallFrameReturn> EvaluateOnCallFrame(EvaluateOnCallFrameParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.evaluateOnCallFrame", parameters, sessionId);
            return Convert<EvaluateOnCallFrameReturn>(___r);
        }
        /// <summary> 
        /// Returns possible locations for breakpoint. scriptId in start and end range locations should bethe same. 
        /// </summary>
        public async Task<GetPossibleBreakpointsReturn> GetPossibleBreakpoints(GetPossibleBreakpointsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.getPossibleBreakpoints", parameters, sessionId);
            return Convert<GetPossibleBreakpointsReturn>(___r);
        }
        /// <summary> 
        /// Returns source for the script with given id. 
        /// </summary>
        public async Task<GetScriptSourceReturn> GetScriptSource(GetScriptSourceParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.getScriptSource", parameters, sessionId);
            return Convert<GetScriptSourceReturn>(___r);
        }
        /// <summary> 
        /// This command is deprecated. Use getScriptSource instead. 
        /// </summary>
        public async Task<GetWasmBytecodeReturn> GetWasmBytecode(GetWasmBytecodeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.getWasmBytecode", parameters, sessionId);
            return Convert<GetWasmBytecodeReturn>(___r);
        }
        /// <summary> 
        /// Returns stack trace with given `stackTraceId`. 
        /// </summary>
        public async Task<GetStackTraceReturn> GetStackTrace(GetStackTraceParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.getStackTrace", parameters, sessionId);
            return Convert<GetStackTraceReturn>(___r);
        }
        /// <summary> 
        /// Stops on the next JavaScript statement. 
        /// </summary>
        public async Task Pause(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.pause", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task PauseOnAsyncCall(PauseOnAsyncCallParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.pauseOnAsyncCall", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Removes JavaScript breakpoint. 
        /// </summary>
        public async Task RemoveBreakpoint(RemoveBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.removeBreakpoint", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Restarts particular call frame from the beginning. 
        /// </summary>
        public async Task<RestartFrameReturn> RestartFrame(RestartFrameParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.restartFrame", parameters, sessionId);
            return Convert<RestartFrameReturn>(___r);
        }
        /// <summary> 
        /// Resumes JavaScript execution. 
        /// </summary>
        public async Task Resume(ResumeParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.resume", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Searches for given string in script content. 
        /// </summary>
        public async Task<SearchInContentReturn> SearchInContent(SearchInContentParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.searchInContent", parameters, sessionId);
            return Convert<SearchInContentReturn>(___r);
        }
        /// <summary> 
        /// Enables or disables async call stacks tracking. 
        /// </summary>
        public async Task SetAsyncCallStackDepth(SetAsyncCallStackDepthParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setAsyncCallStackDepth", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Replace previous blackbox patterns with passed ones. Forces backend to skip stepping/pausing inscripts with url matching one of the patterns. VM will try to leave blackboxed script byperforming 'step in' several times, finally resorting to 'step out' if unsuccessful. 
        /// </summary>
        public async Task SetBlackboxPatterns(SetBlackboxPatternsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setBlackboxPatterns", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Makes backend skip steps in the script in blackboxed ranges. VM will try leave blacklistedscripts by performing 'step in' several times, finally resorting to 'step out' if unsuccessful.Positions array contains positions where blackbox state is changed. First interval isn'tblackboxed. Array should be sorted. 
        /// </summary>
        public async Task SetBlackboxedRanges(SetBlackboxedRangesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setBlackboxedRanges", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Sets JavaScript breakpoint at a given location. 
        /// </summary>
        public async Task<SetBreakpointReturn> SetBreakpoint(SetBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setBreakpoint", parameters, sessionId);
            return Convert<SetBreakpointReturn>(___r);
        }
        /// <summary> 
        /// Sets instrumentation breakpoint. 
        /// </summary>
        public async Task<SetInstrumentationBreakpointReturn> SetInstrumentationBreakpoint(SetInstrumentationBreakpointParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setInstrumentationBreakpoint", parameters, sessionId);
            return Convert<SetInstrumentationBreakpointReturn>(___r);
        }
        /// <summary> 
        /// Sets JavaScript breakpoint at given location specified either by URL or URL regex. Once thiscommand is issued, all existing parsed scripts will have breakpoints resolved and returned in`locations` property. Further matching script parsing will result in subsequent`breakpointResolved` events issued. This logical breakpoint will survive page reloads. 
        /// </summary>
        public async Task<SetBreakpointByUrlReturn> SetBreakpointByUrl(SetBreakpointByUrlParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setBreakpointByUrl", parameters, sessionId);
            return Convert<SetBreakpointByUrlReturn>(___r);
        }
        /// <summary> 
        /// Sets JavaScript breakpoint before each call to the given function.If another function was created from the same source as a given one,calling it will also trigger the breakpoint. 
        /// </summary>
        public async Task<SetBreakpointOnFunctionCallReturn> SetBreakpointOnFunctionCall(SetBreakpointOnFunctionCallParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setBreakpointOnFunctionCall", parameters, sessionId);
            return Convert<SetBreakpointOnFunctionCallReturn>(___r);
        }
        /// <summary> 
        /// Activates / deactivates all breakpoints on the page. 
        /// </summary>
        public async Task SetBreakpointsActive(SetBreakpointsActiveParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setBreakpointsActive", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Defines pause on exceptions state. Can be set to stop on all exceptions, uncaught exceptions orno exceptions. Initial pause on exceptions state is `none`. 
        /// </summary>
        public async Task SetPauseOnExceptions(SetPauseOnExceptionsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setPauseOnExceptions", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Changes return value in top frame. Available only at return break position. 
        /// </summary>
        public async Task SetReturnValue(SetReturnValueParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setReturnValue", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Edits JavaScript source live. 
        /// </summary>
        public async Task<SetScriptSourceReturn> SetScriptSource(SetScriptSourceParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setScriptSource", parameters, sessionId);
            return Convert<SetScriptSourceReturn>(___r);
        }
        /// <summary> 
        /// Makes page not interrupt on any pauses (breakpoint, exception, dom exception etc). 
        /// </summary>
        public async Task SetSkipAllPauses(SetSkipAllPausesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setSkipAllPauses", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Changes value of variable in a callframe. Object-based scopes are not supported and must bemutated manually. 
        /// </summary>
        public async Task SetVariableValue(SetVariableValueParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.setVariableValue", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Steps into the function call. 
        /// </summary>
        public async Task StepInto(StepIntoParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.stepInto", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Steps out of the function call. 
        /// </summary>
        public async Task StepOut(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.stepOut", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Steps over the statement. 
        /// </summary>
        public async Task StepOver(StepOverParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Debugger.stepOver", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class LocationType
        {
            
            /// <summary> Script identifier as reported in the `Debugger.scriptParsed`. </summary>
            public string scriptId;
            /// <summary> Line number in the script (0-based). </summary>
            public int lineNumber;
            /// <summary> Column number in the script (0-based). </summary>
            public int columnNumber;
        }
        public class ScriptPositionType
        {
            
            /// <summary>  </summary>
            public int lineNumber;
            /// <summary>  </summary>
            public int columnNumber;
        }
        public class LocationRangeType
        {
            
            /// <summary>  </summary>
            public string scriptId;
            /// <summary>  </summary>
            public Debugger.ScriptPositionType start;
            /// <summary>  </summary>
            public Debugger.ScriptPositionType end;
        }
        public class CallFrameType
        {
            
            /// <summary> Call frame identifier. This identifier is only valid while the virtual machine is paused. </summary>
            public string callFrameId;
            /// <summary> Name of the JavaScript function called on this call frame. </summary>
            public string functionName;
            /// <summary> Location in the source code. </summary>
            public Debugger.LocationType functionLocation;
            /// <summary> Location in the source code. </summary>
            public Debugger.LocationType location;
            /// <summary> JavaScript script name or url. </summary>
            public string url;
            /// <summary> Scope chain for this call frame. </summary>
            public object[] scopeChain;
            /// <summary> `this` object for this call frame. </summary>
            public Runtime.RemoteObjectType @this;
            /// <summary> The value being returned, if the function is at return point. </summary>
            public Runtime.RemoteObjectType returnValue;
        }
        public class ScopeType
        {
            
            /// <summary> Scope type. </summary>
            public string type;
            /// <summary> Object representing the scope. For `global` and `with` scopes it represents the actualobject; for the rest of the scopes, it is artificial transient object enumerating scopevariables as its properties. </summary>
            public Runtime.RemoteObjectType @object;
            /// <summary>  </summary>
            public string name;
            /// <summary> Location in the source code where scope starts </summary>
            public Debugger.LocationType startLocation;
            /// <summary> Location in the source code where scope ends </summary>
            public Debugger.LocationType endLocation;
        }
        public class SearchMatchType
        {
            
            /// <summary> Line number in resource content. </summary>
            public double lineNumber;
            /// <summary> Line with match content. </summary>
            public string lineContent;
        }
        public class BreakLocationType
        {
            
            /// <summary> Script identifier as reported in the `Debugger.scriptParsed`. </summary>
            public string scriptId;
            /// <summary> Line number in the script (0-based). </summary>
            public int lineNumber;
            /// <summary> Column number in the script (0-based). </summary>
            public int columnNumber;
            /// <summary>  </summary>
            public string type;
        }
        public class DebugSymbolsType
        {
            
            /// <summary> Type of the debug symbols. </summary>
            public string type;
            /// <summary> URL of the external symbol source. </summary>
            public string externalURL;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnBreakpointResolvedParameters
        {
            
            /// <summary> [Require] Breakpoint unique identifier. </summary>
            public string breakpointId;
            /// <summary> [Require] Actual breakpoint location. </summary>
            public Debugger.LocationType location;
        }
        public class OnPausedParameters
        {
            
            /// <summary> [Require] Call stack the virtual machine stopped on. </summary>
            public object[] callFrames;
            /// <summary> [Require] Pause reason. </summary>
            public string reason;
            /// <summary> [Optional] Object containing break-specific auxiliary properties. </summary>
            public object data;
            /// <summary> [Optional] Hit breakpoints IDs </summary>
            public object[] hitBreakpoints;
            /// <summary> [Optional] Async stack trace, if any. </summary>
            public Runtime.StackTraceType asyncStackTrace;
            /// <summary> [Optional] Async stack trace, if any. </summary>
            public Runtime.StackTraceIdType asyncStackTraceId;
            /// <summary> [Optional] Never present, will be removed. </summary>
            public Runtime.StackTraceIdType asyncCallStackTraceId;
        }
        public class OnScriptFailedToParseParameters
        {
            
            /// <summary> [Require] Identifier of the script parsed. </summary>
            public string scriptId;
            /// <summary> [Require] URL or name of the script parsed (if any). </summary>
            public string url;
            /// <summary> [Require] Line offset of the script within the resource with given URL (for script tags). </summary>
            public int startLine;
            /// <summary> [Require] Column offset of the script within the resource with given URL. </summary>
            public int startColumn;
            /// <summary> [Require] Last line of the script. </summary>
            public int endLine;
            /// <summary> [Require] Length of the last line of the script. </summary>
            public int endColumn;
            /// <summary> [Require] Specifies script creation context. </summary>
            public int executionContextId;
            /// <summary> [Require] Content hash of the script. </summary>
            public string hash;
            /// <summary> [Optional] Embedder-specific auxiliary data. </summary>
            public object executionContextAuxData;
            /// <summary> [Optional] URL of source map associated with script (if any). </summary>
            public string sourceMapURL;
            /// <summary> [Optional] True, if this script has sourceURL. </summary>
            public bool hasSourceURL;
            /// <summary> [Optional] True, if this script is ES6 module. </summary>
            public bool isModule;
            /// <summary> [Optional] This script length. </summary>
            public int length;
            /// <summary> [Optional] JavaScript top stack frame of where the script parsed event was triggered if available. </summary>
            public Runtime.StackTraceType stackTrace;
            /// <summary> [Optional] If the scriptLanguage is WebAssembly, the code section offset in the module. </summary>
            public int codeOffset;
            /// <summary> [Optional] The language of the script. </summary>
            public string scriptLanguage;
            /// <summary> [Optional] The name the embedder supplied for this script. </summary>
            public string embedderName;
        }
        public class OnScriptParsedParameters
        {
            
            /// <summary> [Require] Identifier of the script parsed. </summary>
            public string scriptId;
            /// <summary> [Require] URL or name of the script parsed (if any). </summary>
            public string url;
            /// <summary> [Require] Line offset of the script within the resource with given URL (for script tags). </summary>
            public int startLine;
            /// <summary> [Require] Column offset of the script within the resource with given URL. </summary>
            public int startColumn;
            /// <summary> [Require] Last line of the script. </summary>
            public int endLine;
            /// <summary> [Require] Length of the last line of the script. </summary>
            public int endColumn;
            /// <summary> [Require] Specifies script creation context. </summary>
            public int executionContextId;
            /// <summary> [Require] Content hash of the script. </summary>
            public string hash;
            /// <summary> [Optional] Embedder-specific auxiliary data. </summary>
            public object executionContextAuxData;
            /// <summary> [Optional] True, if this script is generated as a result of the live edit operation. </summary>
            public bool isLiveEdit;
            /// <summary> [Optional] URL of source map associated with script (if any). </summary>
            public string sourceMapURL;
            /// <summary> [Optional] True, if this script has sourceURL. </summary>
            public bool hasSourceURL;
            /// <summary> [Optional] True, if this script is ES6 module. </summary>
            public bool isModule;
            /// <summary> [Optional] This script length. </summary>
            public int length;
            /// <summary> [Optional] JavaScript top stack frame of where the script parsed event was triggered if available. </summary>
            public Runtime.StackTraceType stackTrace;
            /// <summary> [Optional] If the scriptLanguage is WebAssembly, the code section offset in the module. </summary>
            public int codeOffset;
            /// <summary> [Optional] The language of the script. </summary>
            public string scriptLanguage;
            /// <summary> [Optional] If the scriptLanguage is WebASsembly, the source of debug symbols for the module. </summary>
            public Debugger.DebugSymbolsType debugSymbols;
            /// <summary> [Optional] The name the embedder supplied for this script. </summary>
            public string embedderName;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class ContinueToLocationParameters
        {
            
            /// <summary> [Require] Location to continue to. </summary>
            public Debugger.LocationType location;
            /// <summary> [Optional]  </summary>
            public string targetCallFrames;
        }
        public class EnableParameters
        {
            
            /// <summary> [Optional] The maximum size in bytes of collected scripts (not referenced by other heap objects)the debugger can hold. Puts no limit if parameter is omitted. </summary>
            public double maxScriptsCacheSize;
        }
        public class EvaluateOnCallFrameParameters
        {
            
            /// <summary> [Require] Call frame identifier to evaluate on. </summary>
            public string callFrameId;
            /// <summary> [Require] Expression to evaluate. </summary>
            public string expression;
            /// <summary> [Optional] String object group name to put result into (allows rapid releasing resulting object handlesusing `releaseObjectGroup`). </summary>
            public string objectGroup;
            /// <summary> [Optional] Specifies whether command line API should be available to the evaluated expression, defaultsto false. </summary>
            public bool includeCommandLineAPI;
            /// <summary> [Optional] In silent mode exceptions thrown during evaluation are not reported and do not pauseexecution. Overrides `setPauseOnException` state. </summary>
            public bool silent;
            /// <summary> [Optional] Whether the result is expected to be a JSON object that should be sent by value. </summary>
            public bool returnByValue;
            /// <summary> [Optional] Whether preview should be generated for the result. </summary>
            public bool generatePreview;
            /// <summary> [Optional] Whether to throw an exception if side effect cannot be ruled out during evaluation. </summary>
            public bool throwOnSideEffect;
            /// <summary> [Optional] Terminate execution after timing out (number of milliseconds). </summary>
            public double timeout;
        }
        public class GetPossibleBreakpointsParameters
        {
            
            /// <summary> [Require] Start of range to search possible breakpoint locations in. </summary>
            public Debugger.LocationType start;
            /// <summary> [Optional] End of range to search possible breakpoint locations in (excluding). When not specified, endof scripts is used as end of range. </summary>
            public Debugger.LocationType end;
            /// <summary> [Optional] Only consider locations which are in the same (non-nested) function as start. </summary>
            public bool restrictToFunction;
        }
        public class GetScriptSourceParameters
        {
            
            /// <summary> [Require] Id of the script to get source for. </summary>
            public string scriptId;
        }
        public class GetWasmBytecodeParameters
        {
            
            /// <summary> [Require] Id of the Wasm script to get source for. </summary>
            public string scriptId;
        }
        public class GetStackTraceParameters
        {
            
            /// <summary> [Require]  </summary>
            public Runtime.StackTraceIdType stackTraceId;
        }
        public class PauseOnAsyncCallParameters
        {
            
            /// <summary> [Require] Debugger will pause when async call with given stack trace is started. </summary>
            public Runtime.StackTraceIdType parentStackTraceId;
        }
        public class RemoveBreakpointParameters
        {
            
            /// <summary> [Require]  </summary>
            public string breakpointId;
        }
        public class RestartFrameParameters
        {
            
            /// <summary> [Require] Call frame identifier to evaluate on. </summary>
            public string callFrameId;
        }
        public class ResumeParameters
        {
            
            /// <summary> [Optional] Set to true to terminate execution upon resuming execution. In contrastto Runtime.terminateExecution, this will allows to execute furtherJavaScript (i.e. via evaluation) until execution of the paused codeis actually resumed, at which point termination is triggered.If execution is currently not paused, this parameter has no effect. </summary>
            public bool terminateOnResume;
        }
        public class SearchInContentParameters
        {
            
            /// <summary> [Require] Id of the script to search in. </summary>
            public string scriptId;
            /// <summary> [Require] String to search for. </summary>
            public string query;
            /// <summary> [Optional] If true, search is case sensitive. </summary>
            public bool caseSensitive;
            /// <summary> [Optional] If true, treats string parameter as regex. </summary>
            public bool isRegex;
        }
        public class SetAsyncCallStackDepthParameters
        {
            
            /// <summary> [Require] Maximum depth of async call stacks. Setting to `0` will effectively disable collecting asynccall stacks (default). </summary>
            public int maxDepth;
        }
        public class SetBlackboxPatternsParameters
        {
            
            /// <summary> [Require] Array of regexps that will be used to check script url for blackbox state. </summary>
            public object[] patterns;
        }
        public class SetBlackboxedRangesParameters
        {
            
            /// <summary> [Require] Id of the script. </summary>
            public string scriptId;
            /// <summary> [Require]  </summary>
            public object[] positions;
        }
        public class SetBreakpointParameters
        {
            
            /// <summary> [Require] Location to set breakpoint in. </summary>
            public Debugger.LocationType location;
            /// <summary> [Optional] Expression to use as a breakpoint condition. When specified, debugger will only stop on thebreakpoint if this expression evaluates to true. </summary>
            public string condition;
        }
        public class SetInstrumentationBreakpointParameters
        {
            
            /// <summary> [Require] Instrumentation name. </summary>
            public string instrumentation;
        }
        public class SetBreakpointByUrlParameters
        {
            
            /// <summary> [Require] Line number to set breakpoint at. </summary>
            public int lineNumber;
            /// <summary> [Optional] URL of the resources to set breakpoint on. </summary>
            public string url;
            /// <summary> [Optional] Regex pattern for the URLs of the resources to set breakpoints on. Either `url` or`urlRegex` must be specified. </summary>
            public string urlRegex;
            /// <summary> [Optional] Script hash of the resources to set breakpoint on. </summary>
            public string scriptHash;
            /// <summary> [Optional] Offset in the line to set breakpoint at. </summary>
            public int columnNumber;
            /// <summary> [Optional] Expression to use as a breakpoint condition. When specified, debugger will only stop on thebreakpoint if this expression evaluates to true. </summary>
            public string condition;
        }
        public class SetBreakpointOnFunctionCallParameters
        {
            
            /// <summary> [Require] Function object id. </summary>
            public string objectId;
            /// <summary> [Optional] Expression to use as a breakpoint condition. When specified, debugger willstop on the breakpoint if this expression evaluates to true. </summary>
            public string condition;
        }
        public class SetBreakpointsActiveParameters
        {
            
            /// <summary> [Require] New value for breakpoints active state. </summary>
            public bool active;
        }
        public class SetPauseOnExceptionsParameters
        {
            
            /// <summary> [Require] Pause on exceptions mode. </summary>
            public string state;
        }
        public class SetReturnValueParameters
        {
            
            /// <summary> [Require] New return value. </summary>
            public Runtime.CallArgumentType newValue;
        }
        public class SetScriptSourceParameters
        {
            
            /// <summary> [Require] Id of the script to edit. </summary>
            public string scriptId;
            /// <summary> [Require] New content of the script. </summary>
            public string scriptSource;
            /// <summary> [Optional] If true the change will not actually be applied. Dry run may be used to get resultdescription without actually modifying the code. </summary>
            public bool dryRun;
        }
        public class SetSkipAllPausesParameters
        {
            
            /// <summary> [Require] New value for skip pauses state. </summary>
            public bool skip;
        }
        public class SetVariableValueParameters
        {
            
            /// <summary> [Require] 0-based number of scope as was listed in scope chain. Only 'local', 'closure' and 'catch'scope types are allowed. Other scopes could be manipulated manually. </summary>
            public int scopeNumber;
            /// <summary> [Require] Variable name. </summary>
            public string variableName;
            /// <summary> [Require] New variable value. </summary>
            public Runtime.CallArgumentType newValue;
            /// <summary> [Require] Id of callframe that holds variable. </summary>
            public string callFrameId;
        }
        public class StepIntoParameters
        {
            
            /// <summary> [Optional] Debugger will pause on the execution of the first async task which was scheduledbefore next pause. </summary>
            public bool breakOnAsyncCall;
            /// <summary> [Optional] The skipList specifies location ranges that should be skipped on step into. </summary>
            public object[] skipList;
        }
        public class StepOverParameters
        {
            
            /// <summary> [Optional] The skipList specifies location ranges that should be skipped on step over. </summary>
            public object[] skipList;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class EnableReturn
        {
            
            /// <summary> Unique identifier of the debugger. </summary>
            public string debuggerId;
        }
        public class EvaluateOnCallFrameReturn
        {
            
            /// <summary> Object wrapper for the evaluation result. </summary>
            public Runtime.RemoteObjectType result;
            /// <summary> Exception details. </summary>
            public Runtime.ExceptionDetailsType exceptionDetails;
        }
        public class GetPossibleBreakpointsReturn
        {
            
            /// <summary> List of the possible breakpoint locations. </summary>
            public object[] locations;
        }
        public class GetScriptSourceReturn
        {
            
            /// <summary> Script source (empty in case of Wasm bytecode). </summary>
            public string scriptSource;
            /// <summary> Wasm bytecode. (Encoded as a base64 string when passed over JSON) </summary>
            public string bytecode;
        }
        public class GetWasmBytecodeReturn
        {
            
            /// <summary> Script source. (Encoded as a base64 string when passed over JSON) </summary>
            public string bytecode;
        }
        public class GetStackTraceReturn
        {
            
            /// <summary>  </summary>
            public Runtime.StackTraceType stackTrace;
        }
        public class RestartFrameReturn
        {
            
            /// <summary> New stack trace. </summary>
            public object[] callFrames;
            /// <summary> Async stack trace, if any. </summary>
            public Runtime.StackTraceType asyncStackTrace;
            /// <summary> Async stack trace, if any. </summary>
            public Runtime.StackTraceIdType asyncStackTraceId;
        }
        public class SearchInContentReturn
        {
            
            /// <summary> List of search matches. </summary>
            public object[] result;
        }
        public class SetBreakpointReturn
        {
            
            /// <summary> Id of the created breakpoint for further reference. </summary>
            public string breakpointId;
            /// <summary> Location this breakpoint resolved into. </summary>
            public Debugger.LocationType actualLocation;
        }
        public class SetInstrumentationBreakpointReturn
        {
            
            /// <summary> Id of the created breakpoint for further reference. </summary>
            public string breakpointId;
        }
        public class SetBreakpointByUrlReturn
        {
            
            /// <summary> Id of the created breakpoint for further reference. </summary>
            public string breakpointId;
            /// <summary> List of the locations this breakpoint resolved into upon addition. </summary>
            public object[] locations;
        }
        public class SetBreakpointOnFunctionCallReturn
        {
            
            /// <summary> Id of the created breakpoint for further reference. </summary>
            public string breakpointId;
        }
        public class SetScriptSourceReturn
        {
            
            /// <summary> New stack trace in case editing has happened while VM was stopped. </summary>
            public object[] callFrames;
            /// <summary> Whether current call stack  was modified after applying the changes. </summary>
            public bool stackChanged;
            /// <summary> Async stack trace, if any. </summary>
            public Runtime.StackTraceType asyncStackTrace;
            /// <summary> Async stack trace, if any. </summary>
            public Runtime.StackTraceIdType asyncStackTraceId;
            /// <summary> Exception details if any. </summary>
            public Runtime.ExceptionDetailsType exceptionDetails;
        }
    }
    
    public class HeapProfiler : DomainBase
    {
        public HeapProfiler(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnAddHeapSnapshotChunk(Action<OnAddHeapSnapshotChunkParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnAddHeapSnapshotChunkParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "HeapProfiler.addHeapSnapshotChunk" : $"HeapProfiler.addHeapSnapshotChunk.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> If heap objects tracking has been started then backend may send update for one or more fragments </summary>
        /// <returns> remove handler </returns>
        public Action OnHeapStatsUpdate(Action<OnHeapStatsUpdateParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnHeapStatsUpdateParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "HeapProfiler.heapStatsUpdate" : $"HeapProfiler.heapStatsUpdate.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> If heap objects tracking has been started then backend regularly sends a current value for lastseen object id and corresponding timestamp. If the were changes in the heap since last eventthen one or more heapStatsUpdate events will be sent before a new lastSeenObjectId event. </summary>
        /// <returns> remove handler </returns>
        public Action OnLastSeenObjectId(Action<OnLastSeenObjectIdParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnLastSeenObjectIdParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "HeapProfiler.lastSeenObjectId" : $"HeapProfiler.lastSeenObjectId.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnReportHeapSnapshotProgress(Action<OnReportHeapSnapshotProgressParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnReportHeapSnapshotProgressParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "HeapProfiler.reportHeapSnapshotProgress" : $"HeapProfiler.reportHeapSnapshotProgress.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnResetProfiles(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "HeapProfiler.resetProfiles" : $"HeapProfiler.resetProfiles.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Enables console to refer to the node with given id via $x (see Command Line API for more details$x functions). 
        /// </summary>
        public async Task AddInspectedHeapObject(AddInspectedHeapObjectParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.addInspectedHeapObject", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task CollectGarbage(string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.collectGarbage", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetHeapObjectIdReturn> GetHeapObjectId(GetHeapObjectIdParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.getHeapObjectId", parameters, sessionId);
            return Convert<GetHeapObjectIdReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetObjectByHeapObjectIdReturn> GetObjectByHeapObjectId(GetObjectByHeapObjectIdParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.getObjectByHeapObjectId", parameters, sessionId);
            return Convert<GetObjectByHeapObjectIdReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<GetSamplingProfileReturn> GetSamplingProfile(string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.getSamplingProfile", null, sessionId);
            return Convert<GetSamplingProfileReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task StartSampling(StartSamplingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.startSampling", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task StartTrackingHeapObjects(StartTrackingHeapObjectsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.startTrackingHeapObjects", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<StopSamplingReturn> StopSampling(string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.stopSampling", null, sessionId);
            return Convert<StopSamplingReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task StopTrackingHeapObjects(StopTrackingHeapObjectsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.stopTrackingHeapObjects", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task TakeHeapSnapshot(TakeHeapSnapshotParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("HeapProfiler.takeHeapSnapshot", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class SamplingHeapProfileNodeType
        {
            
            /// <summary> Function location. </summary>
            public Runtime.CallFrameType callFrame;
            /// <summary> Allocations size in bytes for the node excluding children. </summary>
            public double selfSize;
            /// <summary> Node id. Ids are unique across all profiles collected between startSampling and stopSampling. </summary>
            public int id;
            /// <summary> Child nodes. </summary>
            public object[] children;
        }
        public class SamplingHeapProfileSampleType
        {
            
            /// <summary> Allocation size in bytes attributed to the sample. </summary>
            public double size;
            /// <summary> Id of the corresponding profile tree node. </summary>
            public int nodeId;
            /// <summary> Time-ordered sample ordinal number. It is unique across all profiles retrievedbetween startSampling and stopSampling. </summary>
            public double ordinal;
        }
        public class SamplingHeapProfileType
        {
            
            /// <summary>  </summary>
            public HeapProfiler.SamplingHeapProfileNodeType head;
            /// <summary>  </summary>
            public object[] samples;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnAddHeapSnapshotChunkParameters
        {
            
            /// <summary> [Require]  </summary>
            public string chunk;
        }
        public class OnHeapStatsUpdateParameters
        {
            
            /// <summary> [Require] An array of triplets. Each triplet describes a fragment. The first integer is the fragmentindex, the second integer is a total count of objects for the fragment, the third integer isa total size of the objects for the fragment. </summary>
            public object[] statsUpdate;
        }
        public class OnLastSeenObjectIdParameters
        {
            
            /// <summary> [Require]  </summary>
            public int lastSeenObjectId;
            /// <summary> [Require]  </summary>
            public double timestamp;
        }
        public class OnReportHeapSnapshotProgressParameters
        {
            
            /// <summary> [Require]  </summary>
            public int done;
            /// <summary> [Require]  </summary>
            public int total;
            /// <summary> [Optional]  </summary>
            public bool finished;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class AddInspectedHeapObjectParameters
        {
            
            /// <summary> [Require] Heap snapshot object id to be accessible by means of $x command line API. </summary>
            public string heapObjectId;
        }
        public class GetHeapObjectIdParameters
        {
            
            /// <summary> [Require] Identifier of the object to get heap object id for. </summary>
            public string objectId;
        }
        public class GetObjectByHeapObjectIdParameters
        {
            
            /// <summary> [Require]  </summary>
            public string objectId;
            /// <summary> [Optional] Symbolic group name that can be used to release multiple objects. </summary>
            public string objectGroup;
        }
        public class StartSamplingParameters
        {
            
            /// <summary> [Optional] Average sample interval in bytes. Poisson distribution is used for the intervals. Thedefault value is 32768 bytes. </summary>
            public double samplingInterval;
        }
        public class StartTrackingHeapObjectsParameters
        {
            
            /// <summary> [Optional]  </summary>
            public bool trackAllocations;
        }
        public class StopTrackingHeapObjectsParameters
        {
            
            /// <summary> [Optional] If true 'reportHeapSnapshotProgress' events will be generated while snapshot is being takenwhen the tracking is stopped. </summary>
            public bool reportProgress;
            /// <summary> [Optional]  </summary>
            public bool treatGlobalObjectsAsRoots;
            /// <summary> [Optional] If true, numerical values are included in the snapshot </summary>
            public bool captureNumericValue;
        }
        public class TakeHeapSnapshotParameters
        {
            
            /// <summary> [Optional] If true 'reportHeapSnapshotProgress' events will be generated while snapshot is being taken. </summary>
            public bool reportProgress;
            /// <summary> [Optional] If true, a raw snapshot without artificial roots will be generated </summary>
            public bool treatGlobalObjectsAsRoots;
            /// <summary> [Optional] If true, numerical values are included in the snapshot </summary>
            public bool captureNumericValue;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetHeapObjectIdReturn
        {
            
            /// <summary> Id of the heap snapshot object corresponding to the passed remote object id. </summary>
            public string heapSnapshotObjectId;
        }
        public class GetObjectByHeapObjectIdReturn
        {
            
            /// <summary> Evaluation result. </summary>
            public Runtime.RemoteObjectType result;
        }
        public class GetSamplingProfileReturn
        {
            
            /// <summary> Return the sampling profile being collected. </summary>
            public HeapProfiler.SamplingHeapProfileType profile;
        }
        public class StopSamplingReturn
        {
            
            /// <summary> Recorded sampling heap profile. </summary>
            public HeapProfiler.SamplingHeapProfileType profile;
        }
    }
    
    public class Profiler : DomainBase
    {
        public Profiler(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary>  </summary>
        /// <returns> remove handler </returns>
        public Action OnConsoleProfileFinished(Action<OnConsoleProfileFinishedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnConsoleProfileFinishedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Profiler.consoleProfileFinished" : $"Profiler.consoleProfileFinished.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Sent when new profile recording is started using console.profile() call. </summary>
        /// <returns> remove handler </returns>
        public Action OnConsoleProfileStarted(Action<OnConsoleProfileStartedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnConsoleProfileStartedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Profiler.consoleProfileStarted" : $"Profiler.consoleProfileStarted.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Reports coverage delta since the last poll (either from an event like this, or from`takePreciseCoverage` for the current isolate. May only be sent if precise codecoverage has been started. This event can be trigged by the embedder to, for example,trigger collection of coverage data immediately at a certain point in time. </summary>
        /// <returns> remove handler </returns>
        public Action OnPreciseCoverageDeltaUpdate(Action<OnPreciseCoverageDeltaUpdateParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnPreciseCoverageDeltaUpdateParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Profiler.preciseCoverageDeltaUpdate" : $"Profiler.preciseCoverageDeltaUpdate.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        ///  
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Collect coverage data for the current isolate. The coverage data may be incomplete due togarbage collection. 
        /// </summary>
        public async Task<GetBestEffortCoverageReturn> GetBestEffortCoverage(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.getBestEffortCoverage", null, sessionId);
            return Convert<GetBestEffortCoverageReturn>(___r);
        }
        /// <summary> 
        /// Changes CPU profiler sampling interval. Must be called before CPU profiles recording started. 
        /// </summary>
        public async Task SetSamplingInterval(SetSamplingIntervalParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.setSamplingInterval", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task Start(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.start", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enable precise code coverage. Coverage data for JavaScript executed before enabling precise codecoverage may be incomplete. Enabling prevents running optimized code and resets executioncounters. 
        /// </summary>
        public async Task<StartPreciseCoverageReturn> StartPreciseCoverage(StartPreciseCoverageParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.startPreciseCoverage", parameters, sessionId);
            return Convert<StartPreciseCoverageReturn>(___r);
        }
        /// <summary> 
        /// Enable type profile. 
        /// </summary>
        public async Task StartTypeProfile(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.startTypeProfile", null, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<StopReturn> Stop(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.stop", null, sessionId);
            return Convert<StopReturn>(___r);
        }
        /// <summary> 
        /// Disable precise code coverage. Disabling releases unnecessary execution count records and allowsexecuting optimized code. 
        /// </summary>
        public async Task StopPreciseCoverage(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.stopPreciseCoverage", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Disable type profile. Disabling releases type profile data collected so far. 
        /// </summary>
        public async Task StopTypeProfile(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.stopTypeProfile", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Collect coverage data for the current isolate, and resets execution counters. Precise codecoverage needs to have started. 
        /// </summary>
        public async Task<TakePreciseCoverageReturn> TakePreciseCoverage(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.takePreciseCoverage", null, sessionId);
            return Convert<TakePreciseCoverageReturn>(___r);
        }
        /// <summary> 
        /// Collect type profile. 
        /// </summary>
        public async Task<TakeTypeProfileReturn> TakeTypeProfile(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Profiler.takeTypeProfile", null, sessionId);
            return Convert<TakeTypeProfileReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class ProfileNodeType
        {
            
            /// <summary> Unique id of the node. </summary>
            public int id;
            /// <summary> Function location. </summary>
            public Runtime.CallFrameType callFrame;
            /// <summary> Number of samples where this node was on top of the call stack. </summary>
            public int hitCount;
            /// <summary> Child node ids. </summary>
            public object[] children;
            /// <summary> The reason of being not optimized. The function may be deoptimized or marked as don'toptimize. </summary>
            public string deoptReason;
            /// <summary> An array of source position ticks. </summary>
            public object[] positionTicks;
        }
        public class ProfileType
        {
            
            /// <summary> The list of profile nodes. First item is the root node. </summary>
            public object[] nodes;
            /// <summary> Profiling start timestamp in microseconds. </summary>
            public double startTime;
            /// <summary> Profiling end timestamp in microseconds. </summary>
            public double endTime;
            /// <summary> Ids of samples top nodes. </summary>
            public object[] samples;
            /// <summary> Time intervals between adjacent samples in microseconds. The first delta is relative to theprofile startTime. </summary>
            public object[] timeDeltas;
        }
        public class PositionTickInfoType
        {
            
            /// <summary> Source line number (1-based). </summary>
            public int line;
            /// <summary> Number of samples attributed to the source line. </summary>
            public int ticks;
        }
        public class CoverageRangeType
        {
            
            /// <summary> JavaScript script source offset for the range start. </summary>
            public int startOffset;
            /// <summary> JavaScript script source offset for the range end. </summary>
            public int endOffset;
            /// <summary> Collected execution count of the source range. </summary>
            public int count;
        }
        public class FunctionCoverageType
        {
            
            /// <summary> JavaScript function name. </summary>
            public string functionName;
            /// <summary> Source ranges inside the function with coverage data. </summary>
            public object[] ranges;
            /// <summary> Whether coverage data for this function has block granularity. </summary>
            public bool isBlockCoverage;
        }
        public class ScriptCoverageType
        {
            
            /// <summary> JavaScript script id. </summary>
            public string scriptId;
            /// <summary> JavaScript script name or url. </summary>
            public string url;
            /// <summary> Functions contained in the script that has coverage data. </summary>
            public object[] functions;
        }
        public class TypeObjectType
        {
            
            /// <summary> Name of a type collected with type profiling. </summary>
            public string name;
        }
        public class TypeProfileEntryType
        {
            
            /// <summary> Source offset of the parameter or end of function for return values. </summary>
            public int offset;
            /// <summary> The types for this parameter or return value. </summary>
            public object[] types;
        }
        public class ScriptTypeProfileType
        {
            
            /// <summary> JavaScript script id. </summary>
            public string scriptId;
            /// <summary> JavaScript script name or url. </summary>
            public string url;
            /// <summary> Type profile entries for parameters and return values of the functions in the script. </summary>
            public object[] entries;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnConsoleProfileFinishedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string id;
            /// <summary> [Require] Location of console.profileEnd(). </summary>
            public Debugger.LocationType location;
            /// <summary> [Require]  </summary>
            public Profiler.ProfileType profile;
            /// <summary> [Optional] Profile title passed as an argument to console.profile(). </summary>
            public string title;
        }
        public class OnConsoleProfileStartedParameters
        {
            
            /// <summary> [Require]  </summary>
            public string id;
            /// <summary> [Require] Location of console.profile(). </summary>
            public Debugger.LocationType location;
            /// <summary> [Optional] Profile title passed as an argument to console.profile(). </summary>
            public string title;
        }
        public class OnPreciseCoverageDeltaUpdateParameters
        {
            
            /// <summary> [Require] Monotonically increasing time (in seconds) when the coverage update was taken in the backend. </summary>
            public double timestamp;
            /// <summary> [Require] Identifier for distinguishing coverage events. </summary>
            public string occasion;
            /// <summary> [Require] Coverage data for the current isolate. </summary>
            public object[] result;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class SetSamplingIntervalParameters
        {
            
            /// <summary> [Require] New sampling interval in microseconds. </summary>
            public int interval;
        }
        public class StartPreciseCoverageParameters
        {
            
            /// <summary> [Optional] Collect accurate call counts beyond simple 'covered' or 'not covered'. </summary>
            public bool callCount;
            /// <summary> [Optional] Collect block-based coverage. </summary>
            public bool detailed;
            /// <summary> [Optional] Allow the backend to send updates on its own initiative </summary>
            public bool allowTriggeredUpdates;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetBestEffortCoverageReturn
        {
            
            /// <summary> Coverage data for the current isolate. </summary>
            public object[] result;
        }
        public class StartPreciseCoverageReturn
        {
            
            /// <summary> Monotonically increasing time (in seconds) when the coverage update was taken in the backend. </summary>
            public double timestamp;
        }
        public class StopReturn
        {
            
            /// <summary> Recorded profile. </summary>
            public Profiler.ProfileType profile;
        }
        public class TakePreciseCoverageReturn
        {
            
            /// <summary> Coverage data for the current isolate. </summary>
            public object[] result;
            /// <summary> Monotonically increasing time (in seconds) when the coverage update was taken in the backend. </summary>
            public double timestamp;
        }
        public class TakeTypeProfileReturn
        {
            
            /// <summary> Type profile for all scripts since startTypeProfile() was turned on. </summary>
            public object[] result;
        }
    }
    
    public class Runtime : DomainBase
    {
        public Runtime(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        
        /// <summary> Notification is issued every time when binding is called. </summary>
        /// <returns> remove handler </returns>
        public Action OnBindingCalled(Action<OnBindingCalledParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnBindingCalledParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Runtime.bindingCalled" : $"Runtime.bindingCalled.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when console API was called. </summary>
        /// <returns> remove handler </returns>
        public Action OnConsoleAPICalled(Action<OnConsoleAPICalledParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnConsoleAPICalledParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Runtime.consoleAPICalled" : $"Runtime.consoleAPICalled.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when unhandled exception was revoked. </summary>
        /// <returns> remove handler </returns>
        public Action OnExceptionRevoked(Action<OnExceptionRevokedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnExceptionRevokedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Runtime.exceptionRevoked" : $"Runtime.exceptionRevoked.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when exception was thrown and unhandled. </summary>
        /// <returns> remove handler </returns>
        public Action OnExceptionThrown(Action<OnExceptionThrownParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnExceptionThrownParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Runtime.exceptionThrown" : $"Runtime.exceptionThrown.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when new execution context is created. </summary>
        /// <returns> remove handler </returns>
        public Action OnExecutionContextCreated(Action<OnExecutionContextCreatedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnExecutionContextCreatedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Runtime.executionContextCreated" : $"Runtime.executionContextCreated.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when execution context is destroyed. </summary>
        /// <returns> remove handler </returns>
        public Action OnExecutionContextDestroyed(Action<OnExecutionContextDestroyedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnExecutionContextDestroyedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Runtime.executionContextDestroyed" : $"Runtime.executionContextDestroyed.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        
        /// <summary> Issued when all executionContexts were cleared in browser </summary>
        /// <returns> remove handler </returns>
        public Action OnExecutionContextsCleared(Action handler, string sessionId = default)
        {
            string eventName = string.IsNullOrEmpty(sessionId) ? "Runtime.executionContextsCleared" : $"Runtime.executionContextsCleared.{sessionId}";
            this.chrome.On(eventName, handler);
            return () => this.chrome.Remove(eventName, handler);
        }
        
        /// <summary> Issued when object should be inspected (for example, as a result of inspect() command line APIcall). </summary>
        /// <returns> remove handler </returns>
        public Action OnInspectRequested(Action<OnInspectRequestedParameters> handler, string sessionId = default)
        {
            Action<Dictionary<string, object>> _handler = (d) =>
            {
                handler(Convert<OnInspectRequestedParameters>(d));
            };
            string eventName = string.IsNullOrEmpty(sessionId) ? "Runtime.inspectRequested" : $"Runtime.inspectRequested.{sessionId}";
            this.chrome.On(eventName, _handler);
            return () => this.chrome.Remove(eventName, _handler);
        }
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Add handler to promise with given promise object id. 
        /// </summary>
        public async Task<AwaitPromiseReturn> AwaitPromise(AwaitPromiseParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.awaitPromise", parameters, sessionId);
            return Convert<AwaitPromiseReturn>(___r);
        }
        /// <summary> 
        /// Calls function with given declaration on the given object. Object group of the result isinherited from the target object. 
        /// </summary>
        public async Task<CallFunctionOnReturn> CallFunctionOn(CallFunctionOnParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.callFunctionOn", parameters, sessionId);
            return Convert<CallFunctionOnReturn>(___r);
        }
        /// <summary> 
        /// Compiles expression. 
        /// </summary>
        public async Task<CompileScriptReturn> CompileScript(CompileScriptParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.compileScript", parameters, sessionId);
            return Convert<CompileScriptReturn>(___r);
        }
        /// <summary> 
        /// Disables reporting of execution contexts creation. 
        /// </summary>
        public async Task Disable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.disable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Discards collected exceptions and console API calls. 
        /// </summary>
        public async Task DiscardConsoleEntries(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.discardConsoleEntries", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Enables reporting of execution contexts creation by means of `executionContextCreated` event.When the reporting gets enabled the event will be sent immediately for each existing executioncontext. 
        /// </summary>
        public async Task Enable(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.enable", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Evaluates expression on global object. 
        /// </summary>
        public async Task<EvaluateReturn> Evaluate(EvaluateParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.evaluate", parameters, sessionId);
            return Convert<EvaluateReturn>(___r);
        }
        /// <summary> 
        /// Returns the isolate id. 
        /// </summary>
        public async Task<GetIsolateIdReturn> GetIsolateId(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.getIsolateId", null, sessionId);
            return Convert<GetIsolateIdReturn>(___r);
        }
        /// <summary> 
        /// Returns the JavaScript heap usage.It is the total usage of the corresponding isolate not scoped to a particular Runtime. 
        /// </summary>
        public async Task<GetHeapUsageReturn> GetHeapUsage(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.getHeapUsage", null, sessionId);
            return Convert<GetHeapUsageReturn>(___r);
        }
        /// <summary> 
        /// Returns properties of a given object. Object group of the result is inherited from the targetobject. 
        /// </summary>
        public async Task<GetPropertiesReturn> GetProperties(GetPropertiesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.getProperties", parameters, sessionId);
            return Convert<GetPropertiesReturn>(___r);
        }
        /// <summary> 
        /// Returns all let, const and class variables from global scope. 
        /// </summary>
        public async Task<GlobalLexicalScopeNamesReturn> GlobalLexicalScopeNames(GlobalLexicalScopeNamesParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.globalLexicalScopeNames", parameters, sessionId);
            return Convert<GlobalLexicalScopeNamesReturn>(___r);
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task<QueryObjectsReturn> QueryObjects(QueryObjectsParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.queryObjects", parameters, sessionId);
            return Convert<QueryObjectsReturn>(___r);
        }
        /// <summary> 
        /// Releases remote object with given id. 
        /// </summary>
        public async Task ReleaseObject(ReleaseObjectParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.releaseObject", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Releases all remote objects that belong to a given group. 
        /// </summary>
        public async Task ReleaseObjectGroup(ReleaseObjectGroupParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.releaseObjectGroup", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Tells inspected instance to run if it was waiting for debugger to attach. 
        /// </summary>
        public async Task RunIfWaitingForDebugger(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.runIfWaitingForDebugger", null, sessionId);
            return ;
        }
        /// <summary> 
        /// Runs script with given id in a given context. 
        /// </summary>
        public async Task<RunScriptReturn> RunScript(RunScriptParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.runScript", parameters, sessionId);
            return Convert<RunScriptReturn>(___r);
        }
        /// <summary> 
        /// Enables or disables async call stacks tracking. 
        /// </summary>
        public async Task SetAsyncCallStackDepth(SetAsyncCallStackDepthParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.setAsyncCallStackDepth", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetCustomObjectFormatterEnabled(SetCustomObjectFormatterEnabledParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.setCustomObjectFormatterEnabled", parameters, sessionId);
            return ;
        }
        /// <summary> 
        ///  
        /// </summary>
        public async Task SetMaxCallStackSizeToCapture(SetMaxCallStackSizeToCaptureParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.setMaxCallStackSizeToCapture", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// Terminate current or next JavaScript execution.Will cancel the termination when the outer-most script execution ends. 
        /// </summary>
        public async Task TerminateExecution(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.terminateExecution", null, sessionId);
            return ;
        }
        /// <summary> 
        /// If executionContextId is empty, adds binding with the given name on theglobal objects of all inspected contexts, including those created later,bindings survive reloads.Binding function takes exactly one argument, this argument should be string,in case of any other input, function throws an exception.Each binding function call produces Runtime.bindingCalled notification. 
        /// </summary>
        public async Task AddBinding(AddBindingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.addBinding", parameters, sessionId);
            return ;
        }
        /// <summary> 
        /// This method does not remove binding function from global object butunsubscribes current runtime agent from Runtime.bindingCalled notifications. 
        /// </summary>
        public async Task RemoveBinding(RemoveBindingParameters parameters, string sessionId = default)
        {
            var ___r = await this.chrome.Send("Runtime.removeBinding", parameters, sessionId);
            return ;
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class RemoteObjectType
        {
            
            /// <summary> Object type. </summary>
            public string type;
            /// <summary> Object subtype hint. Specified for `object` type values only.NOTE: If you change anything here, make sure to also update`subtype` in `ObjectPreview` and `PropertyPreview` below. </summary>
            public string subtype;
            /// <summary> Object class (constructor) name. Specified for `object` type values only. </summary>
            public string className;
            /// <summary> Remote object value in case of primitive values or JSON values (if it was requested). </summary>
            public object value;
            /// <summary> Primitive value which can not be JSON-stringified does not have `value`, but gets thisproperty. </summary>
            public string unserializableValue;
            /// <summary> String representation of the object. </summary>
            public string description;
            /// <summary> Unique object identifier (for non-primitive values). </summary>
            public string objectId;
            /// <summary> Preview containing abbreviated property values. Specified for `object` type values only. </summary>
            public Runtime.ObjectPreviewType preview;
            /// <summary>  </summary>
            public Runtime.CustomPreviewType customPreview;
        }
        public class CustomPreviewType
        {
            
            /// <summary> The JSON-stringified result of formatter.header(object, config) call.It contains json ML array that represents RemoteObject. </summary>
            public string header;
            /// <summary> If formatter returns true as a result of formatter.hasBody call then bodyGetterId willcontain RemoteObjectId for the function that returns result of formatter.body(object, config) call.The result value is json ML array. </summary>
            public string bodyGetterId;
        }
        public class ObjectPreviewType
        {
            
            /// <summary> Object type. </summary>
            public string type;
            /// <summary> Object subtype hint. Specified for `object` type values only. </summary>
            public string subtype;
            /// <summary> String representation of the object. </summary>
            public string description;
            /// <summary> True iff some of the properties or entries of the original object did not fit. </summary>
            public bool overflow;
            /// <summary> List of the properties. </summary>
            public object[] properties;
            /// <summary> List of the entries. Specified for `map` and `set` subtype values only. </summary>
            public object[] entries;
        }
        public class PropertyPreviewType
        {
            
            /// <summary> Property name. </summary>
            public string name;
            /// <summary> Object type. Accessor means that the property itself is an accessor property. </summary>
            public string type;
            /// <summary> User-friendly property value string. </summary>
            public string value;
            /// <summary> Nested value preview. </summary>
            public Runtime.ObjectPreviewType valuePreview;
            /// <summary> Object subtype hint. Specified for `object` type values only. </summary>
            public string subtype;
        }
        public class EntryPreviewType
        {
            
            /// <summary> Preview of the key. Specified for map-like collection entries. </summary>
            public Runtime.ObjectPreviewType key;
            /// <summary> Preview of the value. </summary>
            public Runtime.ObjectPreviewType value;
        }
        public class PropertyDescriptorType
        {
            
            /// <summary> Property name or symbol description. </summary>
            public string name;
            /// <summary> The value associated with the property. </summary>
            public Runtime.RemoteObjectType value;
            /// <summary> True if the value associated with the property may be changed (data descriptors only). </summary>
            public bool writable;
            /// <summary> A function which serves as a getter for the property, or `undefined` if there is no getter(accessor descriptors only). </summary>
            public Runtime.RemoteObjectType get;
            /// <summary> A function which serves as a setter for the property, or `undefined` if there is no setter(accessor descriptors only). </summary>
            public Runtime.RemoteObjectType set;
            /// <summary> True if the type of this property descriptor may be changed and if the property may bedeleted from the corresponding object. </summary>
            public bool configurable;
            /// <summary> True if this property shows up during enumeration of the properties on the correspondingobject. </summary>
            public bool enumerable;
            /// <summary> True if the result was thrown during the evaluation. </summary>
            public bool wasThrown;
            /// <summary> True if the property is owned for the object. </summary>
            public bool isOwn;
            /// <summary> Property symbol object, if the property is of the `symbol` type. </summary>
            public Runtime.RemoteObjectType symbol;
        }
        public class InternalPropertyDescriptorType
        {
            
            /// <summary> Conventional property name. </summary>
            public string name;
            /// <summary> The value associated with the property. </summary>
            public Runtime.RemoteObjectType value;
        }
        public class PrivatePropertyDescriptorType
        {
            
            /// <summary> Private property name. </summary>
            public string name;
            /// <summary> The value associated with the private property. </summary>
            public Runtime.RemoteObjectType value;
            /// <summary> A function which serves as a getter for the private property,or `undefined` if there is no getter (accessor descriptors only). </summary>
            public Runtime.RemoteObjectType get;
            /// <summary> A function which serves as a setter for the private property,or `undefined` if there is no setter (accessor descriptors only). </summary>
            public Runtime.RemoteObjectType set;
        }
        public class CallArgumentType
        {
            
            /// <summary> Primitive value or serializable javascript object. </summary>
            public object value;
            /// <summary> Primitive value which can not be JSON-stringified. </summary>
            public string unserializableValue;
            /// <summary> Remote object handle. </summary>
            public string objectId;
        }
        public class ExecutionContextDescriptionType
        {
            
            /// <summary> Unique id of the execution context. It can be used to specify in which execution contextscript evaluation should be performed. </summary>
            public int id;
            /// <summary> Execution context origin. </summary>
            public string origin;
            /// <summary> Human readable name describing given context. </summary>
            public string name;
            /// <summary> A system-unique execution context identifier. Unlike the id, this is unique acrossmultiple processes, so can be reliably used to identify specific context while backendperforms a cross-process navigation. </summary>
            public string uniqueId;
            /// <summary> Embedder-specific auxiliary data. </summary>
            public object auxData;
        }
        public class ExceptionDetailsType
        {
            
            /// <summary> Exception id. </summary>
            public int exceptionId;
            /// <summary> Exception text, which should be used together with exception object when available. </summary>
            public string text;
            /// <summary> Line number of the exception location (0-based). </summary>
            public int lineNumber;
            /// <summary> Column number of the exception location (0-based). </summary>
            public int columnNumber;
            /// <summary> Script ID of the exception location. </summary>
            public string scriptId;
            /// <summary> URL of the exception location, to be used when the script was not reported. </summary>
            public string url;
            /// <summary> JavaScript stack trace if available. </summary>
            public Runtime.StackTraceType stackTrace;
            /// <summary> Exception object if available. </summary>
            public Runtime.RemoteObjectType exception;
            /// <summary> Identifier of the context where exception happened. </summary>
            public int executionContextId;
            /// <summary> Dictionary with entries of meta data that the client associatedwith this exception, such as information about associated networkrequests, etc. </summary>
            public object exceptionMetaData;
        }
        public class CallFrameType
        {
            
            /// <summary> JavaScript function name. </summary>
            public string functionName;
            /// <summary> JavaScript script id. </summary>
            public string scriptId;
            /// <summary> JavaScript script name or url. </summary>
            public string url;
            /// <summary> JavaScript script line number (0-based). </summary>
            public int lineNumber;
            /// <summary> JavaScript script column number (0-based). </summary>
            public int columnNumber;
        }
        public class StackTraceType
        {
            
            /// <summary> String label of this stack trace. For async traces this may be a name of the function thatinitiated the async call. </summary>
            public string description;
            /// <summary> JavaScript function name. </summary>
            public object[] callFrames;
            /// <summary> Asynchronous JavaScript stack trace that preceded this stack, if available. </summary>
            public Runtime.StackTraceType parent;
            /// <summary> Asynchronous JavaScript stack trace that preceded this stack, if available. </summary>
            public Runtime.StackTraceIdType parentId;
        }
        public class StackTraceIdType
        {
            
            /// <summary>  </summary>
            public string id;
            /// <summary>  </summary>
            public string debuggerId;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        
        public class OnBindingCalledParameters
        {
            
            /// <summary> [Require]  </summary>
            public string name;
            /// <summary> [Require]  </summary>
            public string payload;
            /// <summary> [Require] Identifier of the context where the call was made. </summary>
            public int executionContextId;
        }
        public class OnConsoleAPICalledParameters
        {
            
            /// <summary> [Require] Type of the call. </summary>
            public string type;
            /// <summary> [Require] Call arguments. </summary>
            public object[] args;
            /// <summary> [Require] Identifier of the context where the call was made. </summary>
            public int executionContextId;
            /// <summary> [Require] Call timestamp. </summary>
            public double timestamp;
            /// <summary> [Optional] Stack trace captured when the call was made. The async stack chain is automatically reported forthe following call types: `assert`, `error`, `trace`, `warning`. For other types the async callchain can be retrieved using `Debugger.getStackTrace` and `stackTrace.parentId` field. </summary>
            public Runtime.StackTraceType stackTrace;
            /// <summary> [Optional] Console context descriptor for calls on non-default console context (not console.*):'anonymous#unique-logger-id' for call on unnamed context, 'name#unique-logger-id' for callon named context. </summary>
            public string context;
        }
        public class OnExceptionRevokedParameters
        {
            
            /// <summary> [Require] Reason describing why exception was revoked. </summary>
            public string reason;
            /// <summary> [Require] The id of revoked exception, as reported in `exceptionThrown`. </summary>
            public int exceptionId;
        }
        public class OnExceptionThrownParameters
        {
            
            /// <summary> [Require] Timestamp of the exception. </summary>
            public double timestamp;
            /// <summary> [Require]  </summary>
            public Runtime.ExceptionDetailsType exceptionDetails;
        }
        public class OnExecutionContextCreatedParameters
        {
            
            /// <summary> [Require] A newly created execution context. </summary>
            public Runtime.ExecutionContextDescriptionType context;
        }
        public class OnExecutionContextDestroyedParameters
        {
            
            /// <summary> [Require] Id of the destroyed context </summary>
            public int executionContextId;
        }
        public class OnInspectRequestedParameters
        {
            
            /// <summary> [Require]  </summary>
            public Runtime.RemoteObjectType @object;
            /// <summary> [Require]  </summary>
            public object hints;
            /// <summary> [Optional] Identifier of the context where the call was made. </summary>
            public int executionContextId;
        }

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        
        public class AwaitPromiseParameters
        {
            
            /// <summary> [Require] Identifier of the promise. </summary>
            public string promiseObjectId;
            /// <summary> [Optional] Whether the result is expected to be a JSON object that should be sent by value. </summary>
            public bool returnByValue;
            /// <summary> [Optional] Whether preview should be generated for the result. </summary>
            public bool generatePreview;
        }
        public class CallFunctionOnParameters
        {
            
            /// <summary> [Require] Declaration of the function to call. </summary>
            public string functionDeclaration;
            /// <summary> [Optional] Identifier of the object to call function on. Either objectId or executionContextId shouldbe specified. </summary>
            public string objectId;
            /// <summary> [Optional] Call arguments. All call arguments must belong to the same JavaScript world as the targetobject. </summary>
            public object[] arguments;
            /// <summary> [Optional] In silent mode exceptions thrown during evaluation are not reported and do not pauseexecution. Overrides `setPauseOnException` state. </summary>
            public bool silent;
            /// <summary> [Optional] Whether the result is expected to be a JSON object which should be sent by value. </summary>
            public bool returnByValue;
            /// <summary> [Optional] Whether preview should be generated for the result. </summary>
            public bool generatePreview;
            /// <summary> [Optional] Whether execution should be treated as initiated by user in the UI. </summary>
            public bool userGesture;
            /// <summary> [Optional] Whether execution should `await` for resulting value and return once awaited promise isresolved. </summary>
            public bool awaitPromise;
            /// <summary> [Optional] Specifies execution context which global object will be used to call function on. EitherexecutionContextId or objectId should be specified. </summary>
            public int executionContextId;
            /// <summary> [Optional] Symbolic group name that can be used to release multiple objects. If objectGroup is notspecified and objectId is, objectGroup will be inherited from object. </summary>
            public string objectGroup;
            /// <summary> [Optional] Whether to throw an exception if side effect cannot be ruled out during evaluation. </summary>
            public bool throwOnSideEffect;
        }
        public class CompileScriptParameters
        {
            
            /// <summary> [Require] Expression to compile. </summary>
            public string expression;
            /// <summary> [Require] Source url to be set for the script. </summary>
            public string sourceURL;
            /// <summary> [Require] Specifies whether the compiled script should be persisted. </summary>
            public bool persistScript;
            /// <summary> [Optional] Specifies in which execution context to perform script run. If the parameter is omitted theevaluation will be performed in the context of the inspected page. </summary>
            public int executionContextId;
        }
        public class EvaluateParameters
        {
            
            /// <summary> [Require] Expression to evaluate. </summary>
            public string expression;
            /// <summary> [Optional] Symbolic group name that can be used to release multiple objects. </summary>
            public string objectGroup;
            /// <summary> [Optional] Determines whether Command Line API should be available during the evaluation. </summary>
            public bool includeCommandLineAPI;
            /// <summary> [Optional] In silent mode exceptions thrown during evaluation are not reported and do not pauseexecution. Overrides `setPauseOnException` state. </summary>
            public bool silent;
            /// <summary> [Optional] Specifies in which execution context to perform evaluation. If the parameter is omitted theevaluation will be performed in the context of the inspected page.This is mutually exclusive with `uniqueContextId`, which offers analternative way to identify the execution context that is more reliablein a multi-process environment. </summary>
            public int contextId;
            /// <summary> [Optional] Whether the result is expected to be a JSON object that should be sent by value. </summary>
            public bool returnByValue;
            /// <summary> [Optional] Whether preview should be generated for the result. </summary>
            public bool generatePreview;
            /// <summary> [Optional] Whether execution should be treated as initiated by user in the UI. </summary>
            public bool userGesture;
            /// <summary> [Optional] Whether execution should `await` for resulting value and return once awaited promise isresolved. </summary>
            public bool awaitPromise;
            /// <summary> [Optional] Whether to throw an exception if side effect cannot be ruled out during evaluation.This implies `disableBreaks` below. </summary>
            public bool throwOnSideEffect;
            /// <summary> [Optional] Terminate execution after timing out (number of milliseconds). </summary>
            public double timeout;
            /// <summary> [Optional] Disable breakpoints during execution. </summary>
            public bool disableBreaks;
            /// <summary> [Optional] Setting this flag to true enables `let` re-declaration and top-level `await`.Note that `let` variables can only be re-declared if they originate from`replMode` themselves. </summary>
            public bool replMode;
            /// <summary> [Optional] The Content Security Policy (CSP) for the target might block 'unsafe-eval'which includes eval(), Function(), setTimeout() and setInterval()when called with non-callable arguments. This flag bypasses CSP for thisevaluation and allows unsafe-eval. Defaults to true. </summary>
            public bool allowUnsafeEvalBlockedByCSP;
            /// <summary> [Optional] An alternative way to specify the execution context to evaluate in.Compared to contextId that may be reused across processes, this is guaranteed to besystem-unique, so it can be used to prevent accidental evaluation of the expressionin context different than intended (e.g. as a result of navigation across processboundaries).This is mutually exclusive with `contextId`. </summary>
            public string uniqueContextId;
        }
        public class GetPropertiesParameters
        {
            
            /// <summary> [Require] Identifier of the object to return properties for. </summary>
            public string objectId;
            /// <summary> [Optional] If true, returns properties belonging only to the element itself, not to its prototypechain. </summary>
            public bool ownProperties;
            /// <summary> [Optional] If true, returns accessor properties (with getter/setter) only; internal properties are notreturned either. </summary>
            public bool accessorPropertiesOnly;
            /// <summary> [Optional] Whether preview should be generated for the results. </summary>
            public bool generatePreview;
            /// <summary> [Optional] If true, returns non-indexed properties only. </summary>
            public bool nonIndexedPropertiesOnly;
        }
        public class GlobalLexicalScopeNamesParameters
        {
            
            /// <summary> [Optional] Specifies in which execution context to lookup global scope variables. </summary>
            public int executionContextId;
        }
        public class QueryObjectsParameters
        {
            
            /// <summary> [Require] Identifier of the prototype to return objects for. </summary>
            public string prototypeObjectId;
            /// <summary> [Optional] Symbolic group name that can be used to release the results. </summary>
            public string objectGroup;
        }
        public class ReleaseObjectParameters
        {
            
            /// <summary> [Require] Identifier of the object to release. </summary>
            public string objectId;
        }
        public class ReleaseObjectGroupParameters
        {
            
            /// <summary> [Require] Symbolic object group name. </summary>
            public string objectGroup;
        }
        public class RunScriptParameters
        {
            
            /// <summary> [Require] Id of the script to run. </summary>
            public string scriptId;
            /// <summary> [Optional] Specifies in which execution context to perform script run. If the parameter is omitted theevaluation will be performed in the context of the inspected page. </summary>
            public int executionContextId;
            /// <summary> [Optional] Symbolic group name that can be used to release multiple objects. </summary>
            public string objectGroup;
            /// <summary> [Optional] In silent mode exceptions thrown during evaluation are not reported and do not pauseexecution. Overrides `setPauseOnException` state. </summary>
            public bool silent;
            /// <summary> [Optional] Determines whether Command Line API should be available during the evaluation. </summary>
            public bool includeCommandLineAPI;
            /// <summary> [Optional] Whether the result is expected to be a JSON object which should be sent by value. </summary>
            public bool returnByValue;
            /// <summary> [Optional] Whether preview should be generated for the result. </summary>
            public bool generatePreview;
            /// <summary> [Optional] Whether execution should `await` for resulting value and return once awaited promise isresolved. </summary>
            public bool awaitPromise;
        }
        public class SetAsyncCallStackDepthParameters
        {
            
            /// <summary> [Require] Maximum depth of async call stacks. Setting to `0` will effectively disable collecting asynccall stacks (default). </summary>
            public int maxDepth;
        }
        public class SetCustomObjectFormatterEnabledParameters
        {
            
            /// <summary> [Require]  </summary>
            public bool enabled;
        }
        public class SetMaxCallStackSizeToCaptureParameters
        {
            
            /// <summary> [Require]  </summary>
            public int size;
        }
        public class AddBindingParameters
        {
            
            /// <summary> [Require]  </summary>
            public string name;
            /// <summary> [Optional] If specified, the binding would only be exposed to the specifiedexecution context. If omitted and `executionContextName` is not set,the binding is exposed to all execution contexts of the target.This parameter is mutually exclusive with `executionContextName`.Deprecated in favor of `executionContextName` due to an unclear use caseand bugs in implementation (crbug.com/1169639). `executionContextId` will beremoved in the future. </summary>
            public int executionContextId;
            /// <summary> [Optional] If specified, the binding is exposed to the executionContext withmatching name, even for contexts created after the binding is added.See also `ExecutionContext.name` and `worldName` parameter to`Page.addScriptToEvaluateOnNewDocument`.This parameter is mutually exclusive with `executionContextId`. </summary>
            public string executionContextName;
        }
        public class RemoveBindingParameters
        {
            
            /// <summary> [Require]  </summary>
            public string name;
        }

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class AwaitPromiseReturn
        {
            
            /// <summary> Promise result. Will contain rejected value if promise was rejected. </summary>
            public Runtime.RemoteObjectType result;
            /// <summary> Exception details if stack strace is available. </summary>
            public Runtime.ExceptionDetailsType exceptionDetails;
        }
        public class CallFunctionOnReturn
        {
            
            /// <summary> Call result. </summary>
            public Runtime.RemoteObjectType result;
            /// <summary> Exception details. </summary>
            public Runtime.ExceptionDetailsType exceptionDetails;
        }
        public class CompileScriptReturn
        {
            
            /// <summary> Id of the script. </summary>
            public string scriptId;
            /// <summary> Exception details. </summary>
            public Runtime.ExceptionDetailsType exceptionDetails;
        }
        public class EvaluateReturn
        {
            
            /// <summary> Evaluation result. </summary>
            public Runtime.RemoteObjectType result;
            /// <summary> Exception details. </summary>
            public Runtime.ExceptionDetailsType exceptionDetails;
        }
        public class GetIsolateIdReturn
        {
            
            /// <summary> The isolate id. </summary>
            public string id;
        }
        public class GetHeapUsageReturn
        {
            
            /// <summary> Used heap size in bytes. </summary>
            public double usedSize;
            /// <summary> Allocated heap size in bytes. </summary>
            public double totalSize;
        }
        public class GetPropertiesReturn
        {
            
            /// <summary> Object properties. </summary>
            public object[] result;
            /// <summary> Internal object properties (only of the element itself). </summary>
            public object[] internalProperties;
            /// <summary> Object private properties. </summary>
            public object[] privateProperties;
            /// <summary> Exception details. </summary>
            public Runtime.ExceptionDetailsType exceptionDetails;
        }
        public class GlobalLexicalScopeNamesReturn
        {
            
            /// <summary>  </summary>
            public object[] names;
        }
        public class QueryObjectsReturn
        {
            
            /// <summary> Array with objects. </summary>
            public Runtime.RemoteObjectType objects;
        }
        public class RunScriptReturn
        {
            
            /// <summary> Run result. </summary>
            public Runtime.RemoteObjectType result;
            /// <summary> Exception details. </summary>
            public Runtime.ExceptionDetailsType exceptionDetails;
        }
    }
    
    public class Schema : DomainBase
    {
        public Schema(CDP.Chrome chrome) : base(chrome) { }

        ///////////////////////////////////////////////////////////
        ///events
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands
        ///////////////////////////////////////////////////////////
        
        /// <summary> 
        /// Returns supported domains. 
        /// </summary>
        public async Task<GetDomainsReturn> GetDomains(string sessionId = default)
        {
            var ___r = await this.chrome.Send("Schema.getDomains", null, sessionId);
            return Convert<GetDomainsReturn>(___r);
        }

        ///////////////////////////////////////////////////////////
        ///types
        ///////////////////////////////////////////////////////////
        
        public class DomainType
        {
            
            /// <summary> Domain name. </summary>
            public string name;
            /// <summary> Domain version. </summary>
            public string version;
        }

        ///////////////////////////////////////////////////////////
        ///event parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands parameters types
        ///////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////
        ///commands return types
        ///////////////////////////////////////////////////////////
        
        public class GetDomainsReturn
        {
            
            /// <summary> List of supported domains. </summary>
            public object[] domains;
        }
    }
    
}
