# 💰 Infrastructure Cost Report

> **Project:** `UPT-FAING-EPIS/proyecto-si784-2025-i-u3-scaeupt/WebApp/pyWeb_ScaeUPT/terraform/plan.json`  
> **Total Monthly Cost:** **`$29.82`**  
> **Generated:** `2025-07-05 17:05:34`

## 📊 Detailed Breakdown

```
Project: UPT-FAING-EPIS/proyecto-si784-2025-i-u3-scaeupt/WebApp/pyWeb_ScaeUPT/terraform/plan.json

 Name                                        Monthly Qty  Unit                    Monthly Cost   
                                                                                                 
 azurerm_service_plan.python                                                                     
 └─ Instance usage (B2)                              730  hours                         $24.82   
                                                                                                 
 azurerm_container_registry.main                                                                 
 ├─ Registry usage (Basic)                            30  days                           $5.00   
 ├─ Storage (over 10GB)                Monthly cost depends on usage: $0.10 per GB               
 └─ Build vCPU                         Monthly cost depends on usage: $0.0001 per seconds        
                                                                                                 
 azurerm_service_plan.main                                                                       
 └─ Instance usage (F1)                              730  hours                          $0.00   
                                                                                                 
 azurerm_application_insights.main                                                               
 └─ Data ingested                      Monthly cost depends on usage: $2.30 per GB               
                                                                                                 
 azurerm_application_insights.python                                                             
 └─ Data ingested                      Monthly cost depends on usage: $2.30 per GB               
                                                                                                 
 azurerm_log_analytics_workspace.main                                                            
 ├─ Log data ingestion                 Monthly cost depends on usage: $2.30 per GB               
 ├─ Log data export                    Monthly cost depends on usage: $0.10 per GB               
 ├─ Basic log data ingestion           Monthly cost depends on usage: $0.50 per GB               
 ├─ Basic log search queries           Monthly cost depends on usage: $0.005 per GB searched     
 ├─ Archive data                       Monthly cost depends on usage: $0.02 per GB               
 ├─ Archive data restored              Monthly cost depends on usage: $0.10 per GB               
 └─ Archive data searched              Monthly cost depends on usage: $0.005 per GB              
                                                                                                 
 azurerm_monitor_action_group.main                                                               
 └─ Email notifications (1)            Monthly cost depends on usage: $0.00002 per emails        
                                                                                                 
 OVERALL TOTAL                                                                         $29.82 

*Usage costs were estimated using infracost-usage.yml, see docs for other options.

──────────────────────────────────
12 cloud resources were detected:
∙ 9 were estimated
∙ 3 were free

┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┳━━━━━━━━━━━━━━━┳━━━━━━━━━━━━━┳━━━━━━━━━━━━┓
┃ Project                                                          ┃ Baseline cost ┃ Usage cost* ┃ Total cost ┃
┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━╋━━━━━━━━━━━━━━━╋━━━━━━━━━━━━━╋━━━━━━━━━━━━┫
┃ UPT-FAING-EPIS/proyecto-si784-2...eb_ScaeUPT/terraform/plan.json ┃           $30 ┃       $0.00 ┃        $30 ┃
┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┻━━━━━━━━━━━━━━━┻━━━━━━━━━━━━━┻━━━━━━━━━━━━┛
```

# ## 📈 Cost Analysis

# >  └─ Archive data searched              Monthly cost depends on usage: $0.005 per GB              
>                                                                                                  
>  azurerm_monitor_action_group.main                                                               
>  └─ Email notifications (1)            Monthly cost depends on usage: $0.00002 per emails        
>                                                                                                  
>  OVERALL TOTAL                                                                         $29.82 
> 
> *Usage costs were estimated using infracost-usage.yml, see docs for other options.
> 
> ──────────────────────────────────
> 12 cloud resources were detected:
> ∙ 9 were estimated
> ∙ 3 were free
> 
> ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━┳━━━━━━━━━━━━━━━┳━━━━━━━━━━━━━┳━━━━━━━━━━━━┓
> ┃ Project                                                          ┃ Baseline cost ┃ Usage cost* ┃ Total cost ┃

---

<details>
<summary>🔧 Technical Details</summary>

- **Tool:** Infracost `v0.10.41`
- **Format:** Infrastructure as Code cost estimation
- **Timestamp:** `2025-07-05T17:05:34Z`

</details>

*This report was automatically generated by the CI/CD pipeline using Infracost.*

## 💾 Additional Manual Costs

| 🏷️ Resource | 💵 Monthly Cost |
|-------------|------------------|
| MySQL Database (Elastika) | **$2.78** |

> **Note:** These are additional costs not tracked by Terraform but included in the total infrastructure budget.

