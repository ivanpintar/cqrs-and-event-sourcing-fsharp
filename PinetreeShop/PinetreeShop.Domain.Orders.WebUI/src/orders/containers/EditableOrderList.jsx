import { connect } from 'react-redux';
import { cancelOrder, shipOrder, deliverOrder } from '../actions';
import OrderList from '../components/OrderList';

const sortOrders = (a,b) => {
    if (a.id < b.id) return -1; 
    if (a.id > b.id) return 1;
    return 0; 
}

const getSortedOrders = (orders) => {
    return orders.sort(sortOrders);    
}

const mapStateToProps = (state) => {
    return {
        orders: getSortedOrders(state.orders)
    };
}

const mapDispatchToProps = (dispatch) => {
    return {
        cancel: (order) => dispatch(cancelOrder(order)),
        ship: (order) => dispatch(shipOrder(order)),
        deliver: (order) => dispatch(deliverOrder(order))        
    };
}

const EditableOrderList = connect(mapStateToProps, mapDispatchToProps)(OrderList);

export default EditableOrderList;