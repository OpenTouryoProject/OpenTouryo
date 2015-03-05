Select TOP(1) UserId,
              ProcessName,
              StatusId,
	      Data,
       	      RegistrationDateTime,
	      ExecutionStartDateTime,
	      NumberOfRetries,
	      CompletionDateTime,
 	      CommandId
       
FROM   AsyncProcessingServiceStatusManagementTable
       [StatusId]=1 
            or 
       [StatusId]=5
  
     
  
