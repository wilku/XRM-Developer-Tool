﻿using JosephM.Application.Desktop.Module.ServiceRequest;
using JosephM.Application.ViewModel.Dialog;
using JosephM.Application.ViewModel.Fakes;
using JosephM.Application.ViewModel.Shared;

namespace JosephM.TestModule.TestDialog
{
    public class TestDialog :
        ServiceRequestDialog<TestDialogService, TestDialogRequest, TestDialogResponse, TestDialogResponseItem>
    {
        public TestDialog(IDialogController dialogController)
            : base(new TestDialogService(), dialogController, FakeRecordService.Get())
        {
            
        }

        protected override void CompleteDialogExtention()
        {

            base.CompleteDialogExtention();

            CompletionOptions.Add(new XrmButtonViewModel("Option 1", () => ApplicationController.UserMessage("Dummy"), ApplicationController));
            CompletionOptions.Add(new XrmButtonViewModel("option 2", () => ApplicationController.UserMessage("Dummy 2"), ApplicationController));
        }
    }
}