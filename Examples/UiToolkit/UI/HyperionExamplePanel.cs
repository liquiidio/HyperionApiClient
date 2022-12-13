using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Packages.UniversalAuthenticatorLibrary.Src.UiToolkit.Ui;
using AtomicAssetsApiClient.Core.Assets;
using AtomicAssetsApiClient.Core.Collections;
using AtomicAssetsApiClient.Core.Exceptions;
using AtomicAssetsApiClient.Unity3D;
using AtomicAssetsApiClient.Unity3D.Assets;
using AtomicAssetsApiClient.Unity3D.Collections;
using HyperionApiClient;
using HyperionApiClient.Clients;
using HyperionApiClient.Responses;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class HyperionExamplePanel : MonoBehaviour
{
    /*
     * Child-Controls
     */
    public VisualElement Root;


    private VisualElement _accountBox;
    private VisualElement _infoBox;
    private VisualElement _blockBox;

    private Label _accountLabel;
    private Label _creatorLabel;
    private Label _timeSpanLabel;
    private Label _blockNumberLabel;
    private Label _trxIdLabel;

    private Label _confirmLabel;
    private Label _previousBlockLabel;
    //private Label _blockCPULimitLabel;
    private Label _timestampBlockLabel;
    private Label _blockNumberBlockLabel;
    private Label _blockActionMrootLabel;
    private Label _blockTransactionMrootLabel;
    private Label _blockProducerSignatureLabel;

    //private Label _headBlockIdLabel;
    private Label _blockCPULimitLabel;
    private Label _blockNETLimitLabel;
    private Label _chainIdLabel;
    private Label _forkDbHeadBlockIdLabel;
    private Label _forkDbHeadBlockNumLabel;
    private Label _headBlockIdLabel;
    private Label _headBlockNumLabel;
    private Label _headBlockProducerLabel;
    private Label _headBlockTimeLabel;

    private Label _filterTypeLabel;

    private Button _searchButton;
    private Button _accountButton;
    private Button _infoButton;
    private Button _blockButton;

    private TextField _textFieldValue;


    /*
     * Fields/Properties
     */

    private AccountsClient _accountsClient;
    private ChainClient _chainClient;

    void Start()
    {
        Root = GetComponent<UIDocument>().rootVisualElement;

        _accountsClient = new AccountsClient(new UnityWebRequestHandler())
        {
            BaseUrl = "https://wax.eosphere.io/"
        };

        _chainClient = new ChainClient(new UnityWebRequestHandler())
        {
            BaseUrl = "https://wax.eosphere.io/"
        };

        _textFieldValue = Root.Q<TextField>("collection-name-or-id-textfield");

        _accountLabel = Root.Q<Label>("account-name-label");
        _creatorLabel = Root.Q<Label>("creator-label");
        _timeSpanLabel = Root.Q<Label>("timespan-label");
        _blockNumberLabel = Root.Q<Label>("block-number-label");
        _trxIdLabel = Root.Q<Label>("transaction-id-label");

        _confirmLabel = Root.Q<Label>("confirm-label");
        _previousBlockLabel = Root.Q<Label>("previous-block-label");
        _timestampBlockLabel = Root.Q<Label>("timestamp-block-label");
        _blockNumberBlockLabel = Root.Q<Label>("block-number-block-label");
        _blockActionMrootLabel = Root.Q<Label>("block-action-mroot-label");
        _blockProducerSignatureLabel = Root.Q<Label>("block-producer-signature-label");
        _blockTransactionMrootLabel = Root.Q<Label>("block-transaction-mroot-label");



        _headBlockIdLabel = Root.Q<Label>("head-block-id-label");
        _blockCPULimitLabel = Root.Q<Label>("block-cpu-limit-label");
        _blockNETLimitLabel = Root.Q<Label>("block-net-limit-label");
        _chainIdLabel = Root.Q<Label>("chain-id-label");
        _forkDbHeadBlockIdLabel = Root.Q<Label>("fork-block-id-label");
        _forkDbHeadBlockNumLabel = Root.Q<Label>("fork-block-number-label");
        _headBlockIdLabel = Root.Q<Label>("head-block-id-label");
        _headBlockNumLabel = Root.Q<Label>("head-block-number-label");
        _headBlockProducerLabel = Root.Q<Label>("head-block-producer-label");
        _headBlockTimeLabel = Root.Q<Label>("head-block-time-label");


        _accountBox = Root.Q<VisualElement>("account-box");
        _blockBox = Root.Q<VisualElement>("block-box");
        _infoBox = Root.Q<VisualElement>("info-box");

        _filterTypeLabel = Root.Q<Label>("filter-type");

        _searchButton = Root.Q<Button>("search-button");
        _accountButton = Root.Q<Button>("account-button");
        _infoButton = Root.Q<Button>("info-button");
        _blockButton = Root.Q<Button>("block-button");


        _filterTypeLabel.text = "Account";

        BindButtons();
    }

    #region Button Binding

    private void BindButtons()
    {
        _searchButton.clicked += SearchAsset;

        _accountButton.clickable.clicked += () =>
        {
            _filterTypeLabel.text = "Account";
            _accountBox.Show();
            _blockBox.Hide(); 
            _infoBox.Hide();
        };

        _blockButton.clickable.clicked += () =>
        {
            _filterTypeLabel.text = "Block";
            _accountBox.Hide();
            _blockBox.Show();
            _infoBox.Hide();
        };

        _infoButton.clickable.clicked += async () =>
        {
            _filterTypeLabel.text = "Info";
            _accountBox.Hide();
            _blockBox.Hide();
            _infoBox.Show();

            var info = await _chainClient.GetInfoAsync();
            Rebind(info);
        };
    }

    #endregion

    #region Rebind
    private void Rebind(GetCreatorResponse creatorResponse)
    {
        _accountLabel.text = creatorResponse.Account;
        _timeSpanLabel.text = creatorResponse.Timestamp;
        _trxIdLabel.text = creatorResponse.TrxId;
        _blockNumberLabel.text = creatorResponse.BlockNum.ToString();
        _creatorLabel.text = creatorResponse.Creator;
    }

    private void Rebind(GetBlockResponse2 blockResponse)
    {
        _blockNumberBlockLabel.text = blockResponse.BlockNum.ToString();
        _blockActionMrootLabel.text = blockResponse.ActionMroot;
        _confirmLabel.text = blockResponse.Confirmed.ToString();
        _previousBlockLabel.text = blockResponse.Previous;
        _blockProducerSignatureLabel.text = blockResponse.Producer;
        _timestampBlockLabel.text = blockResponse.Timestamp;
        _blockTransactionMrootLabel.text = blockResponse.TransactionMroot;
    }

    private void Rebind(GetInfoResponse info)
    {
        _headBlockIdLabel.text = info.HeadBlockId;
        _blockCPULimitLabel.text = info.BlockCpuLimit.ToString();
        _blockNETLimitLabel.text = info.BlockNetLimit.ToString();
        _chainIdLabel.text = info.ChainId;
        _forkDbHeadBlockIdLabel.text = info.ForkDbHeadBlockId;
        _forkDbHeadBlockNumLabel.text = info.ForkDbHeadBlockNum.ToString();
        _headBlockIdLabel.text = info.HeadBlockId;
        _headBlockNumLabel.text = info.HeadBlockNum.ToString();
        _headBlockProducerLabel.text = info.HeadBlockProducer;
        _headBlockTimeLabel.text = info.HeadBlockTime;
    }

    #endregion

    #region Others
    private async void SearchAsset()
    {
        try
        {
            switch (_filterTypeLabel.text)
            {
                case "Account":
                    var account = await _accountsClient.GetCreatorAsync(_textFieldValue.value);
                    if (account != null)
                    {
                        Rebind(account);
                    }
                    else Debug.Log("account not found");
                    break;

                case "Block":
                    var block = await _chainClient.GetBlockAsync(_textFieldValue.value);
                    if (block != null)
                    {
                        Rebind(block);
                    }
                    else Debug.Log("block not found");
                    break;

                case "":
                    break;
            }
        }
        catch (ApiException ex)
        {
            Debug.LogError($"Content: {ex.Content}");
        }
    }
    #endregion
}
