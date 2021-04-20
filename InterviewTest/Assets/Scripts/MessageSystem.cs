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

    public class MessageCore : Singleton<MessageCore>
    {
        //Message Handler Map
        Dictionary<string, Dictionary<int, MessageHandler>> messageHandlersMap = new Dictionary<string, Dictionary<int, MessageHandler>>();

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

        void removeHandler(IMessageSystemHandler handler)
        {

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
                        var methods = handler.Value.HandleMethodMap;
                        if (methods.ContainsKey(msgData.method_id))
                        {
                            var handle_mehtod = methods[msgData.method_id];
                            handle_mehtod(msgData.message_params);
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

        }

        #endregion
    }
}
