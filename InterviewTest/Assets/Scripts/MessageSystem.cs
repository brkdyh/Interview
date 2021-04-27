using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MessageSystem
{
    public delegate void MessageHandleMethod(params object[] message_params);

    public interface IMessageSystemHandler
    {
        string getMessageUid { get; }
        void initHandleMethodMap(Dictionary<string, MessageHandleMethod> HandleMethodMap);
    }

    public class MessageHandler
    {
        public IMessageSystemHandler IHdnaler;
        public Dictionary<string, MessageHandleMethod> HandleMethodMap;

        public int registerObjectHash { get { return IHdnaler.GetHashCode(); } }
        public string messageUid { get { return IHdnaler.getMessageUid; } }

        public MessageHandler(IMessageSystemHandler IHdnaler)
        {
            this.IHdnaler = IHdnaler;
            HandleMethodMap = new Dictionary<string, MessageHandleMethod>();
            IHdnaler.initHandleMethodMap(HandleMethodMap);
        }
    }

    public struct MessageData
    {
        public string message_uid;
        public string method_id;
        public object[] message_params;

        public MessageData(string message_uid, string method_id, object[] message_params)
        {
            this.message_uid = message_uid;
            this.method_id = method_id;
            this.message_params = message_params;
        }
    }

    public class MessageCore : MonoSingleton<MessageCore>
    {
        //Message Handler Map
        Dictionary<string, Dictionary<int, MessageHandler>> messageHandlersMap = new Dictionary<string, Dictionary<int, MessageHandler>>();

        Stack<string> removeHandlers = new Stack<string>();

        void addHandler(IMessageSystemHandler interface_handler)
        {
            string msg_uid = interface_handler.getMessageUid;
            if (!messageHandlersMap.ContainsKey(msg_uid))
            {
                messageHandlersMap.Add(msg_uid, new Dictionary<int, MessageHandler>());
            }

            var handlerDic = messageHandlersMap[msg_uid];
            var handler_hash = interface_handler.GetHashCode();
            if (!handlerDic.ContainsKey(handler_hash))
            {
                MessageHandler handler = new MessageHandler(interface_handler);
                handlerDic.Add(handler_hash, handler);
            }
        }

        void MarkHandlerDispose(IMessageSystemHandler interface_handler)
        {
            lock (removeHandlers)
            {
                removeHandlers.Push(Handler2Identiry(interface_handler));
            }
        }

        void removeHandler(string msg_uid, int handler_hash)
        {
            if (messageHandlersMap.ContainsKey(msg_uid))
            {
                var handlerDic = messageHandlersMap[msg_uid];
                if (handlerDic.ContainsKey(handler_hash))
                {
                    handlerDic.Remove(handler_hash);
                    Debug.Log("Remove Handler <color=#ff0000>" + msg_uid + "+[" + handler_hash + "]</color>");

                }

                if (handlerDic.Count <= 0)
                {
                    messageHandlersMap.Remove(msg_uid);
                    Debug.Log("Remove All Handler, Message Uid is <color=#ffff00>" + msg_uid + "</color>");
                }
            }
        }

        private void LateUpdate()
        {
            while (removeHandlers.Count > 0)
            {
                var rm_handler = removeHandlers.Pop();
                var rm_params = rm_handler.Split('+');
                var rm_msg_uid = rm_params[0];
                var rm_hash = int.Parse(rm_params[1]);
                removeHandler(rm_msg_uid, rm_hash);
            }
        }

        void handleMessage(MessageData msgData)
        {
            string current_handler = "";
            try
            {
                if (messageHandlersMap.ContainsKey(msgData.message_uid))
                {
                    var handlerDic = messageHandlersMap[msgData.message_uid];
                    foreach (var handler in handlerDic)
                    {
                        current_handler = string.Format("{0}+[{1}]", handler.Value.IHdnaler.ToString(), handler.Key);
                        var methods = handler.Value.HandleMethodMap;
                        if (methods.ContainsKey(msgData.method_id))
                        {
                            var handle_mehtod = methods[msgData.method_id];
                            handle_mehtod(msgData.message_params);

                            Debug.Log(string.Format("Handle Message :current handler =  <color=#ff0000>{0}</color> ,msg uid = <color=#ffff00>{1}</color>, method id = <color=#ffff00>{2}</color>",
                            current_handler, msgData.message_uid, msgData.method_id));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(string.Format("Handle Message Exception : current handler = {0} ,msg uid = {1}, method id = {2},\n{3}\n{4}",
                    current_handler, msgData.message_uid, msgData.method_id, ex.Message, ex.StackTrace));
            }
        }

        static string Handler2Identiry(IMessageSystemHandler handler)
        {
            return handler.getMessageUid + "+" + handler.GetHashCode();
        }


        #region Static Methods

        public static void RegisterHandler(IMessageSystemHandler handler)
        {
            Instance.addHandler(handler);
        }

        public static void SendMessage(string msg_uid, string method_id, params object[] msg_params)
        {
            MessageData msgData = new MessageData(msg_uid, method_id, msg_params);
            Instance.handleMessage(msgData);
        }

        public static void UnregisterHandler(IMessageSystemHandler handler)
        {
#if UNITY_EDITOR
            if (Instance == null)
                return;
#endif
            Instance.MarkHandlerDispose(handler);
        }

        #endregion
    }
}
