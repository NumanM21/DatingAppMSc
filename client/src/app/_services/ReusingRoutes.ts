import { ActivatedRouteSnapshot, DetachedRouteHandle, RouteReuseStrategy } from "@angular/router";

// This is a service that is used to reuse routes in the app. (e.g. when a user clicks on a message notification, it will redirect to the message page of the sender user => and update the message thread to read)

export class ReusingRoute implements RouteReuseStrategy {

  // Determines if the current route should be detached and stored for later reuse.
  shouldDetach(route: ActivatedRouteSnapshot): boolean {
    return false;
  }

  // Stores the detached route. This method is called only when `shouldDetach` returns true.
  store(route: ActivatedRouteSnapshot, handle: DetachedRouteHandle | null): void { }

  // Determines if the stored route should be reattached before activating a new route.
  shouldAttach(route: ActivatedRouteSnapshot): boolean {
    return false;
  }

  // Retrieves the stored route. This method is called only when `shouldAttach` returns true.
  retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null {
    return null;
  }

  // Determines if the current route and the future route are the same and should be reused.
  shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean {
    return false;
  }

}