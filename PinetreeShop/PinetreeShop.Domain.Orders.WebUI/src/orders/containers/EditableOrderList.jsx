import { connect } from 'react-redux';
import { cancelOrder, prepareOrderForShipping, shipOrder, deliverOrder } from '../actions';
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
        cancel: (id) => dispatch(cancelOrder(id)),
        prepareForShipping: (id) => dispatch(prepareOrderForShipping(id)),
        ship: (id) => dispatch(shipOrder(id)),
        deliver: (id) => dispatch(deliverOrder(id))        
    };
}

const EditableOrderList = connect(mapStateToProps, mapDispatchToProps)(OrderList);

export default EditableOrderList;